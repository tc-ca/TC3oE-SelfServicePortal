import fs from "fs";
import fetch from "node-fetch";
// load .env file to map
let dotenv = fs.readFileSync(".env").toString();
dotenv = dotenv.split("\n");
dotenv = dotenv.reduce((map, line) => {
	const i = line.indexOf("=");
	const key = line.substring(0, i);
	const value = line.substring(i + 1);
	map[key] = value;
	return map;
}, {})

const useFormBody = false;
const contentType= useFormBody ? "application/x-www-form-urlencoded" : "application/json";

function transformBody(body) {
	if (useFormBody) {
		return new URLSearchParams(body);
	} else {
		return JSON.stringify(body);
	}
}

async function create() {
	const req = await fetch(`https://dev.azure.com/${dotenv.org}/_apis/serviceendpoint/endpoints`, {
		headers: {
			accept: "application/json;api-version=6.0-preview.4;excludeUrls=true;enumsAsNumbers=true;msDateFormat=true;noArrayWrap=true",
			"Content-Type": contentType,
			cookie: dotenv.cookie
		},
		method: "POST",
		body: transformBody(
			{
				"data": {
					"environment": "AzureCloud",
					"scopeLevel": "Subscription",
					"subscriptionId": dotenv.subscriptionId,
					"subscriptionName": "mytestsubscription",
					"resourceGroupName": "",
					"mlWorkspaceName": "",
					"mlWorkspaceLocation": "",
					"managementGroupId": "",
					"managementGroupName": "",
					"oboAuthorization": "",
					"creationMode": "Automatic",
					"azureSpnRoleAssignmentId": "",
					"azureSpnPermissions": "",
					"spnObjectId": "",
					"appObjectId": "",
					"resourceId": ""
				},
				"name": dotenv.name,
				"type": "azurerm",
				"url": "https://management.azure.com/",
				"description": "",
				"authorization": {
					"parameters": {
						// "beans": "sure", // doesn't work
						// "tenantId": dotenv.tenantId, // bad
						"tenantid": dotenv.tenantId, // good
						// "tEnAnTiD": "hehe",
						"serviceprincipalid": "",
						"authenticationType": "spnKey",
						"scope": dotenv.scope,
						"serviceprincipalkey": null
					},
					"scheme": "ServicePrincipal"
				},
				"isShared": false,
				"isReady": false,
				"owner": "library",
				"serviceEndpointProjectReferences": [
					{
						"projectReference": {
							"id": dotenv.projectId,
							"name": dotenv.projectName
						},
						"name": dotenv.name,
						"description": ""
					}
				]
			}
		)
	});
	return await req.json();
}

async function isReady(endpointDetails) {
	const req = await fetch(`https://dev.azure.com/${dotenv.org}/${dotenv.projectId}/_apis/serviceendpoint/endpoints/${endpointDetails.id}`, {
		headers: {
			accept: "application/json;api-version=6.0-preview.4;excludeUrls=true;enumsAsNumbers=true;msDateFormat=true;noArrayWrap=true",
			"Content-Type": contentType,
			cookie: dotenv.cookie
		},
		method: "GET",
	});
	const json = await req.json();
	return json;
}

async function isValid(endpointDetails) {

	const body = transformBody({
		"dataSourceDetails": {
			"dataSourceName": "TestConnection",
			"dataSourceUrl": "",
			"headers": null,
			"resourceUrl": "",
			"requestContent": null,
			"requestVerb": null,
			"parameters": null,
			"resultSelector": "",
			"initialContextTemplate": ""
		},
		"resultTransformationDetails": {
			"callbackContextTemplate": "",
			"callbackRequiredTemplate": "",
			"resultTemplate": ""
		},
		"serviceEndpointDetails": endpointDetails
	});
	const url = `https://dev.azure.com/${dotenv.org}/${dotenv.projectId}/_apis/serviceendpoint/endpointproxy?endpointId=${endpointDetails.id}`;
	const req = await fetch(url, {
		headers: {
			// note that it's preview.1 here instead of preview.4
			accept: "application/json;api-version=6.0-preview.1;excludeUrls=true;enumsAsNumbers=true;msDateFormat=true;noArrayWrap=true",
			"Content-Type": contentType,
			cookie: dotenv.cookie
		},
		method: "POST",
		body
	});
	fs.writeFileSync("asd.json", body);
	if (!req.ok) throw new Error("failed to create: " + req.text());
	const json = await req.json();
	return json;
}

async function main() {
	console.log("Creating...");
	let details = await create();
	console.dir(details);
	const max = 100
	for (let i=0; i<max; i++) {
		console.log("Checking if ready...");
		const x = await isReady(details);
		console.dir(x);
		if (x?.isReady) {
			details = x;
			break;
		} else {
			if (i == max-1) throw new Error(`failed to ready after ${max} tries`);
			await new Promise((res, rej) => setTimeout(res, 3000));
		}
	}
	console.log("Validating...");
	
	for (let i=0; i<30; i++) {
		console.log("Checking if valid...");
		const x = await isValid(details);
		console.dir(x);
		if (x.statusCode == 200) {
			details = x;
			break;
		} else {
			if (i == max-1) throw new Error(`failed to validate after ${max} tries`);
			await new Promise((res, rej) => setTimeout(res, 3000));
		}
	}
}
main()