using Google.Cloud.TextToSpeech.V1;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;

namespace Toeicking2021.Utilities
{
    public class GoogleTTS
    {
        // 檔名腔調轉換
        public static Dictionary<string, string> AccentMapper { get; } = new Dictionary<string, string>
        {
            { "en-US-Wavenet-J","USM"},
            { "en-US-Wavenet-E","USF"},
            { "en-GB-Wavenet-B","GBM"},
            { "en-GB-Wavenet-C","GBF"},
            { "en-AU-Wavenet-B","AUM"},
            { "en-AU-Wavenet-C","AUF"}
        };

        #region 生語音檔
        public static string GenerateVoice(string text, string accent, string rate, string senNum, string webdir) 
        {
            string result = "initial";
            try
            {
                // 初始化一個client端物件
                TextToSpeechClient client = TextToSpeechClient.Create();
                // 傳送要生成語音的文字
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
                // 選擇音檔的類型
                AudioConfig config = new AudioConfig
                {
                    AudioEncoding = AudioEncoding.Mp3,
                    SpeakingRate = double.Parse(rate)
                };
                // 執行Text-to-Speech請求，將所有資料傳送出去
                var response = client.SynthesizeSpeech(new SynthesizeSpeechRequest
                {
                    Input = input,
                    Voice = voice,
                    AudioConfig = config
                });
                // 檔案路徑字串變數
                string FilePath = "";
                // 判斷Runtime的OS以生出正確的檔案路徑
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Windows))
                {
                    FilePath = GenerateLocalFilePath(accent, rate);
                }
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    FilePath = GenerateServerFilePath(webdir, accent, rate, senNum);
                }
                // 生指定路徑的mp3檔，並將聲音寫入
                using (Stream output = File.Create(FilePath))
                {
                    response.AudioContent.WriteTo(output);
                }

                result = "success";

            }
            catch (Exception ex)
            {
                result = ex.InnerException.ToString()+"\n" + ex.Message;      
            }
            return result;


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

        #region 生伺服器檔案路徑字串
        public static string GenerateServerFilePath(string webdir, string accent, string rate, string senNum)
        {
            // 利用傳入的網站資料夾名稱字串生出BaseDirectory路徑字串
            string BaseDirectory = GenerateBaseDirectory(webdir);
            // 再與句子編號合併出新的檔案路徑
            string FilePath = Path.Combine(BaseDirectory, senNum);
            // 建此路徑的資料夾
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            // 再與語速合併出新的檔案路徑
            FilePath = Path.Combine(BaseDirectory, senNum, rate);
            // 建立該編號下的語速資料夾
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            // 判斷語速生出最後的音檔路徑
            if (rate=="0.75")
            {
                // 使用FilePathHelper()去拚出最後的音檔路徑字串
                FilePath = FilePathHelper(FilePath, senNum, accent, rate, BaseDirectory);
            }
            // 字串傳過來是1.0不是1
            else if (rate=="1.0")
            {
                FilePath = FilePathHelper(FilePath, senNum, accent, rate, BaseDirectory);
            }
            else if (rate=="1.25")
            {
                FilePath = FilePathHelper(FilePath, senNum, accent, rate, BaseDirectory);
            }
            return FilePath;

        }
        #endregion
        public static string GenerateBaseDirectory(string webdir) 
        {
            // wwwroot資料夾下的vouce資料夾手動建，更改wwwroot資料夾權限後，其權限也一起更改
            // 但程式裡面也要寫進voice
            return "/var/www/" + webdir + "/wwwroot/voice";
        }

        public static string FilePathHelper(string FilePath, string senNum, string accent, string rate, string BaseDirectory)
        {
            // 檢查語速資料夾是否建立
            if (!Directory.Exists(FilePath))
            {
                Directory.CreateDirectory(FilePath);
            }
            // 拼接檔名字串
            string FileName = senNum + "_" + rate + "_" + AccentMapper[accent] + ".mp3";
            // 將檔名字串加入，生出最終完整檔案路徑
            FilePath = Path.Combine(BaseDirectory, senNum, rate, FileName);
            // 回傳路徑為: /var/www/voice.toeicking.com/wwwroot/voice/1/0.75/1_0.75_AUM.mp3
            return FilePath;

        }



    }

}
