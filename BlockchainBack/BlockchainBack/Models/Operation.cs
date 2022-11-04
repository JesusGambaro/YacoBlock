using System.Text.Json.Serialization;

namespace BlockchainBack.Models;

public class Operation
{
    public string Address { get; set; }
    public string Tipo { get; set; }

    public Operation(string address, string tipo)
    {
        Address = address;
        Tipo = tipo;
    }
    [JsonConstructor]
    public Operation(){}
}