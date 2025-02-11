namespace AppGlobal.Models.Speech;

public static class SpeechVoicesDto
{
    public static Dictionary<string, string> SpeechVoicesList
    {
        get
        {
            return new Dictionary<string, string>
            {
                {"Ryan EN-GB","en-GB-RyanNeural"},
                {"Sonia EN-GB","en-GB-SoniaNeural"},
                {"Aria EN-US","en-US-AriaNeural"},
                {"Davis EN-US","en-US-DavisNeural"},
                {"Guy EN-US","en-US-GuyNeural"},
                {"Jane EN-US","en-US-JaneNeural"},
                {"Jason EN-US","en-US-JasonNeural"},
                {"Jenny EN-US","en-US-JennyNeural"},
                {"Nancy EN-US","en-US-NancyNeural"},
                {"Sara EN-US","en-US-SaraNeural"},
                {"Tony EN-US","en-US-TonyNeural"},
                {"Jorge EN-MX","es-MX-JorgeNeural"},
                {"Denise fr-FR","fr-FR-DeniseNeural"},
                {"Henri fr-FR","fr-FR-HenriNeural"},
                {"Isabella it-IT","it-IT-IsabellaNeural"},
                {"Nanami ja-JP","ja-JP-NanamiNeural"},
                {"Francisca pt-BR","pt-BR-FranciscaNeural"},
                {"Xiaohan zh-CN","zh-CN-XiaohanNeural"},
                {"Xiaomeng zh-CN","zh-CN-XiaomengNeural"},
                {"Xiaomo zh-CN","zh-CN-XiaomoNeural"},
            };
        }
    }
}
