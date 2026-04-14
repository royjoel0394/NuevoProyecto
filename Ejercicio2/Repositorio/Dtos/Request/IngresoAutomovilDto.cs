namespace Ejercicio2.Repositorio.DTOs.Request;

public class CreateIngresoAutomovilDto
{
    public int IdAutomovil { get; set; }
    public int IdParqueo { get; set; }
    public DateTime FechaHoraEntrada { get; set; }
    public DateTime? FechaHoraSalida { get; set; }
}

public class UpdateIngresoAutomovilDto
{
    public DateTime? FechaHoraSalida { get; set; }
}
