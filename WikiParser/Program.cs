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

		public class Config
		{
			public string File;
			public long Start;
			public long StartAuto;
			public long End;
			public int Step;
		}

		public static Config Conf { get; set; }
		public static Config Conf1 { get; set; }

		public static string Read(FileStream r, long start, int len)
		{
			r.Position = start;
			var buff = new byte[len];
			var l = r.Read(buff, 0, len);
			return Encoding.UTF8.GetString(buff, 0, l);
		}

		public static void Main(string[] args)
		{
			LogManager.Configuration.Variables["starttime"] = DateTime.Now.ToString("yyyy-MM-dd HH-mm-ss-ffff");

			using (var sr = new StreamReader("config.json", true))
			{
				Conf = JsonConvert.DeserializeObject<Config>(sr.ReadToEnd());
			}
			Log.Info("Загружена конфигурация");
			Log.Debug($"File = {Conf.File}");
			Log.Debug($"Start =     {Conf.Start}");
			Log.Debug($"StartAuto = {Conf.Start}");
			Log.Debug($"End =       {Conf.End}");
			Log.Debug($"Step =      {Conf.Step}");

			var time = new Stopwatch();

			var wiki = new FileStream(@"\\Netbook-acer\вики\ruwiki-20170401-pages-articles-multistream.xml", FileMode.Open);

			var c = 18219426;
			var a = 18219425131;
			var h = (double)a / c;

			time.Start();
			var s = Read(wiki, 0, c);
			
			var r = new Regex(@"[А-Яа-яЁё]+");
			var m = r.Matches(s);
			var n = m.Count;
			Console.WriteLine($"{n} {time.Elapsed} {time.ElapsedMilliseconds}");
			Console.ReadLine();
		}
	}
}
