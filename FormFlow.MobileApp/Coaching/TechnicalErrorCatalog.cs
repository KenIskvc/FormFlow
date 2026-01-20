namespace FormFlow.MobileApp.Coaching;

public static class TechnicalErrorCatalog
{
    private static readonly Dictionary<string, TechnicalErrorDefinition> _definitions =
        new()
        {
            {
                "KNEE_TOO_STRAIGHT",
                new TechnicalErrorDefinition
                {
                    Code = "KNEE_TOO_STRAIGHT",
                    Title = "Knee not bent enough",
                    Suggestion =
                        "Try bending your knee more during the preparation phase. " +
                        "This helps with shock absorption and improves balance and power transfer."
                }
            }

            // HIER später neue Fehler ergänzen
        };

    public static TechnicalErrorDefinition? Get(string code)
        => _definitions.TryGetValue(code, out var def) ? def : null;
}
