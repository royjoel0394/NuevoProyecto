namespace Ejercicio2.Repositorio.DTOs.Response;

public class IngresoAutomovilDto
{
    public int Consecutivo { get; set; }
    public int IdAutomovil { get; set; }
    public int IdParqueo { get; set; }
    public DateTime FechaHoraEntrada { get; set; }
    public DateTime? FechaHoraSalida { get; set; }
    public int? DuracionEstadiaMinutos { get; set; }
    public decimal? DuracionEstadiaHoras { get; set; }
    public decimal? MontoTotalAPagar { get; set; }
}
