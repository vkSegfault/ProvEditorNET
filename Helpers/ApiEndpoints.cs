namespace ProvEditorNET.Helpers;

public static class ApiEndpoints
{
    public const string ApiBase = "api/v1";

    public static class Counties
    {
        private const string Base = $"{ApiBase}/counties";
        public const string Create = Base;
    }
}