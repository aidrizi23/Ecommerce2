namespace AuthAlbiWebSchool.Models;

public class Response
{
    
    public string? Message { get; set; }
    public bool IsSuccess { get; set; }
    
    public object? Data { get; set; }
}