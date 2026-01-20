using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using FormFlow.MobileApp.DTOs;

namespace FormFlow.MobileApp.Contracts;

public interface IAnalysisApi
{
    // Analyse OHNE Persistenz (Gast oder User)
    Task<AnalysisResponseDto> AnalyzeAsync(
        Stream videoStream,
        string fileName,
        CancellationToken ct);

    // Persistierte Analysen für eingeloggten User
    Task<IReadOnlyList<AnalysisResponseDto>> GetMyAnalysesAsync(
        CancellationToken ct);
    Task DeleteAnalysisAsync(int analysisId, CancellationToken ct);

}
