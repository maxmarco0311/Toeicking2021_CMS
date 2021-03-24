using Google.Cloud.TextToSpeech.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Toeicking2021.Utilities
{
    public class GoogleTTS
    {
        
        #region 生語音檔
        public static void GenerateVoice(string text, string accent, string rate) 
        {
            // Instantiate a client
            TextToSpeechClient client = TextToSpeechClient.Create();

            // Set the text input to be synthesized.
            SynthesisInput input = new SynthesisInput
            {
                Text = text.Trim()
            };

            // Build the voice request, select the language code ("en-US"),
            // and the SSML voice gender ("neutral").
            VoiceSelectionParams voice = new VoiceSelectionParams
            {
                // 必要屬性(從表單值中取出en-US字串)
                LanguageCode = accent.Substring(0, 5), //"en-US",
                Name = accent // en-US-Wavenet-D
            };

            // Select the type of audio file you want returned.
            AudioConfig config = new AudioConfig
            {
                AudioEncoding = AudioEncoding.Mp3,
                SpeakingRate = double.Parse(rate)
            };

            // Perform the Text-to-Speech request, passing the text input
            // with the selected voice parameters and audio file type
            var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
            {
                Input = input,
                Voice = voice,
                AudioConfig = config
            });

            // 生檔案路徑
            string FilePath = GenerateLocalFilePath(accent, rate);

            // Write the binary AudioContent of the response to an MP3 file.
            using (Stream output = File.Create(FilePath))
            {
                response.AudioContent.WriteTo(output);
            }


        }
        #endregion

        #region 生本機檔案路徑字串
        public static string GenerateLocalFilePath(string accent, string rate) 
        {
            // 只要檔案路徑字串變數有出現"\"就要用@
            string BaseDirectory = @"D:\voice\";
            string SubDirectory = DateTime.Now.ToString("yyyyMMdd") + @"\";
            string FileName = rate + "_" + accent + "_" + DateTime.Now.ToString("HHmmss") + ".mp3";
            // 使用Path.Combine()還是要在每個參數字串後加"\"
            string FilePath = Path.Combine(BaseDirectory + SubDirectory + FileName);
            // 檢查音檔所在資料夾是否已存在
            if (!Directory.Exists(Path.Combine(BaseDirectory + SubDirectory)))
            {
                Directory.CreateDirectory(Path.Combine(BaseDirectory + SubDirectory));
            }
            return FilePath;

        }
        #endregion


    }
}
