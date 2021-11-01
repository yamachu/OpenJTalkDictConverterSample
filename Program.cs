using System;
using System.IO;
using System.Linq;
using SharpOpenJTalk;

public class OpenJTalkDictConverterSample
{
    // from: jpcommon_rule_utf_8.h
    static string[] Moras = new[] { "ヴョ", "ヴュ", "ヴャ", "ヴォ", "ヴェ", "ヴィ", "ヴァ", "ヴ", "ン", "ヲ", "ヱ", "ヰ", "ワ", "ヮ", "ロ", "レ", "ル", "リョ", "リュ", "リャ", "リェ", "リ", "ラ", "ヨ", "ョ", "ユ", "ュ", "ヤ", "ャ", "モ", "メ", "ム", "ミョ", "ミュ", "ミャ", "ミェ", "ミ", "マ", "ポ", "ボ", "ホ", "ペ", "ベ", "ヘ", "プ", "ブ", "フォ", "フェ", "フィ", "ファ", "フ", "ピョ", "ピュ", "ピャ", "ピェ", "ピ", "ビョ", "ビュ", "ビャ", "ビェ", "ビ", "ヒョ", "ヒュ", "ヒャ", "ヒェ", "ヒ", "パ", "バ", "ハ", "ノ", "ネ", "ヌ", "ニョ", "ニュ", "ニャ", "ニェ", "ニ", "ナ", "ドゥ", "ド", "トゥ", "ト", "デョ", "デュ", "デャ", "ディ", "デ", "テョ", "テュ", "テャ", "ティ", "テ", "ヅ", "ツォ", "ツェ", "ツィ", "ツァ", "ツ", "ッ", "ヂ", "チョ", "チュ", "チャ", "チェ", "チ", "ダ", "タ", "ゾ", "ソ", "ゼ", "セ", "ズィ", "ズ", "スィ", "ス", "ジョ", "ジュ", "ジャ", "ジェ", "ジ", "ショ", "シュ", "シャ", "シェ", "シ", "ザ", "サ", "ゴ", "コ", "ゲ", "ケ", "ヶ", "グヮ", "グ", "クヮ", "ク", "ギョ", "ギュ", "ギャ", "ギェ", "ギ", "キョ", "キュ", "キャ", "キェ", "キ", "ガ", "カ", "オ", "ォ", "エ", "ェ", "ウォ", "ウェ", "ウィ", "ウ", "ゥ", "イェ", "イ", "ィ", "ア", "ァ", "ー" };

    public static void Main(string[] args)
    {
        var systemDictDir = args[0];
        // https://github.com/neologd/mecab-ipadic-neologd のseedのcsvとかを想定している
        var ipadicCsvFile = args[1];
        var htsVoicePath = args[2];

        var moraRegex = new System.Text.RegularExpressions.Regex(String.Join('|', Moras));
        var tmpFilePath = Path.GetTempFileName();

        var isFirstLine = true;
        using (var f = new StreamWriter(tmpFilePath, false, System.Text.Encoding.UTF8))
        {
            foreach (var line in File.ReadLines(ipadicCsvFile, System.Text.Encoding.UTF8))
            {
                var fields = line.Split(',');
                var moraCount = moraRegex.Matches(fields[12]).Count;

                var newFileds = Enumerable.Concat(fields, new[] { $"1/{moraCount}", "*" });

                if (isFirstLine)
                {
                    // なんか知らんが1行目読まれない………調査する
                    f.Write(String.Join(',', newFileds));
                    f.Write(Environment.NewLine);
                    f.Write(String.Join(',', newFileds));
                    isFirstLine = false;
                    continue;
                }
                f.Write(Environment.NewLine);
                f.Write(String.Join(',', newFileds));
            }
        }
        OpenJTalkAPI.GenerateUserDictionary(systemDictDir, tmpFilePath, Path.Join(systemDictDir, "user.dic"));
        File.Delete(tmpFilePath);

        var openJTalkAPI = new OpenJTalkAPI();
        Console.WriteLine(Path.Join(systemDictDir, "user.dic"));
        Console.WriteLine(openJTalkAPI.Initialize(systemDictDir, htsVoicePath, Path.Join(systemDictDir, "user.dic")));

        var buffer = openJTalkAPI.SynthesisBuffer("GITHUBでコードをホストする");

        var byteWidth = 16 / 8;
        var freq = 48000;

        using (var fs = new FileStream("tmp.wav", FileMode.OpenOrCreate))
        using (var bw = new BinaryWriter(fs))
        {
            bw.Write("RIFF".ToCharArray());
            bw.Write((UInt32)(36 + buffer.Length * byteWidth));
            bw.Write("WAVE".ToCharArray());
            bw.Write("fmt ".ToCharArray());
            bw.Write((UInt32)16);
            bw.Write((UInt16)1);
            bw.Write((UInt16)1);
            bw.Write((UInt32)freq);
            bw.Write((UInt32)(freq * byteWidth));
            bw.Write((UInt16)(byteWidth));
            bw.Write((UInt16)16);
            bw.Write("data".ToCharArray());
            bw.Write((UInt32)(buffer.Length * byteWidth));

            var maxVal = (short)32767;
            var minVal = (short)-32768;

            foreach (var v in buffer)
            {
                if (v > maxVal)
                    bw.Write(maxVal);
                else if (v < minVal)
                    bw.Write(minVal);
                else
                    bw.Write((short)v);
            }
        }
    }
}


