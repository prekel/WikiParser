using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.IO;
using System.Text.RegularExpressions;
using System.Diagnostics;

using Newtonsoft.Json;
using NLog;

namespace WikiParser
{
	public class Program
	{
		private static readonly Logger Log = LogManager.GetCurrentClassLogger();

		public class Configuration
		{
			public string File { get; set; }
			public long Start { get; set; }
			public long StartAuto { get; set; }
			public long End { get; set; }
			public int Step { get; set; }
			public int Reserve { get; set; }
		}

		public Configuration Config { get; set; }

		public static string Read(FileStream fs, long start, int len)
		{
			fs.Position = start;
			var buff = new byte[len];
			var l = fs.Read(buff, 0, len);
			return Encoding.UTF8.GetString(buff, 0, l);
		}

		public HashSet<string> Words { get; set; }

		public FileStream Wiki { get; set; }

		public static List<string> FindStatic(FileStream fs, HashSet<string> words, long start, int len)
		{
			var s = Read(fs, start, len);
			//Log.Debug(s);

			var r = new Regex(@"[А-Яа-яЁё]+");
			var m = r.Matches(s);
			var l = new List<string>();
			foreach (Match j in m)
			{
				//words.Add(ToLower(j.Value));
				var i = ToLower(j.Value);
				if (words.Add(i))
					l.Add(i);
			}
			return l;
		}

		public List<string> Find(long start, int len)
		{
			return FindStatic(Wiki, Words, start, len);
		}

		public static void Main(string[] args)
		{
			new Program();
		}

		public static string ToLower(string s)
		{
			return s.ToLower().Replace('ё', 'е');
		}

		public Program()
		{
			var starttime = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-ffff");
			LogManager.Configuration.Variables["starttime"] = starttime;

			using (var sr = new StreamReader("config.json"))
			{
				Config = JsonConvert.DeserializeObject<Configuration>(sr.ReadToEnd());
			}
			Log.Info("Загружена конфигурация");
			Log.Debug($"File =      {Config.File}");
			Log.Debug($"Start =     {Config.Start}");
			Log.Debug($"StartAuto = {Config.Start}");
			Log.Debug($"End =       {Config.End}");
			Log.Debug($"Step =      {Config.Step}");
			Log.Debug($"Reserve =   {Config.Reserve}");

			Wiki = new FileStream(Config.File, FileMode.Open);
			Words = new HashSet<string>();

			var c = 18219426;
			var a = 18219425131;
			var h = (double)a / c;

			var c1 = 20000000;

			Directory.CreateDirectory("starts");
			Directory.CreateDirectory($"starts\\{starttime}");

			for (var i = Config.StartAuto; i < Config.End; i += Config.Step)
			{
				Log.Trace($"Начат анализ от {i} до {i + Config.Step + Config.Reserve}");
				var f = Find(i, Config.Step + Config.Reserve);
				Log.Trace($"Найдено {f.Count} новых слов, всего {Words.Count}");

				var p = $"starts\\{starttime}\\{i} {i + Config.Step + Config.Reserve}.txt";
				using (var sw = new StreamWriter(p))
				{
					Log.Trace($"Начата запись в {p}");
					foreach (var j in f)
						sw.WriteLine(j);
					Log.Trace($"Закончена запись");
				}
			}

			Log.Info($"Анализ закончен, найдено {Words.Count} слов");

			using (var sw = new StreamWriter("config.json"))
			{
				Config.StartAuto = Config.End;
				sw.Write(JsonConvert.SerializeObject(Config));
			}
		}
	}
}
