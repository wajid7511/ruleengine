using System.ComponentModel.DataAnnotations;

namespace RuleEngine.Example.Dtos;

public class CustomerOrderPostModel
{
    [Required]
    public string Name { get; set; } = string.Empty;
    [Required]
    public CustomerOrderAddressPostModel Address { get; set; } = null!;
    [Required]
    public List<CustomerOrderItemPostModel> Items { get; set; } = null!;
    [Required]
    public string Email { get; set; } = string.Empty;
    [Required]
    public string PhoneNumber { get; set; } = string.Empty;
}

public class CustomerOrderAddressPostModel
{
    [Required]
    public string CompleteAddress { get; set; } = string.Empty;
    public string LandMark { get; set; } = string.Empty;
}

public class CustomerOrderItemPostModel
{
    [Required]
    public int ItemId { get; set; }
    public double Quantity { get; set; }
}