namespace OurAzureDevops.Models;

public record PaginatedResult<T>
{
	public int count {get; init;}
	public T[] value {get; init;}
}

