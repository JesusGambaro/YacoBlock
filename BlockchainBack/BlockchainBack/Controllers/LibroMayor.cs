namespace BlockchainBack.Controllers;

public struct LibroMayor
{
    public string Cuenta { get; set; }
    public decimal Debe { get; set; }
    public List<decimal> DebeList { get; set; }
    public decimal Haber { get; set; }
    public List<decimal> HaberList { get; set; }
    public decimal Saldo { get; set; }
}