namespace FormFlow.Backend.DTOs;

public class AnalysisResponseDto
{
    public DateTime CreatedAt { get; set; }

    // Nur gesetzt, wenn gespeichert (eingeloggt)
    public int? AnalysisId { get; set; }

    public bool IsPersisted => AnalysisId.HasValue;

    // Das eigentliche Analyse-Ergebnis
    public string Report { get; set; } = default!;
}
