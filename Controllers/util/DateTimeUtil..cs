using KintaiAuto.ViewModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace KintaiAuto.Controllers.util
{
    /// <summary>
    /// DateTime型拡張メソッド定義
    /// </summary>
    public static class DateTimeUtil
    {
        private static readonly int FiscalYearStartingMonth = 4;
        private static readonly int HOUTEI_TIME = 9;

        /// <summary>
        /// 該当年月の日数を返す
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <returns>DateTime</returns>
        public static int DaysInMonth(this DateTime dt)
        {
            return DateTime.DaysInMonth(dt.Year, dt.Month);
        }

        /// <summary>
        /// 月初日を返す
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <returns>Datetime</returns>
        public static DateTime BeginOfMonth(this DateTime dt)
        {
            return dt.AddDays((dt.Day - 1) * -1);
        }

        /// <summary>
        /// 月末日を返す
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <returns>DateTime</returns>
        public static DateTime EndOfMonth(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, DaysInMonth(dt));
        }

        /// <summary>
        /// 時刻を落として日付のみにする
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <returns>DateTime</returns>
        public static DateTime StripTime(this DateTime dt)
        {
            return new DateTime(dt.Year, dt.Month, dt.Day);
        }

        /// <summary>
        /// 日付を落として時刻のみにする
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <param name="base_date">DateTime* : 基準日</param>
        /// <returns>DateTime</returns>
        public static DateTime StripDate(this DateTime dt, DateTime? base_date = null)
        {
            base_date = base_date ?? DateTime.MinValue;
            return new DateTime(base_date.Value.Year, base_date.Value.Month, base_date.Value.Day, dt.Hour, dt.Minute, dt.Second);
        }

        /// <summary>
        /// 該当日付の年度を返す
        /// </summary>
        /// <param name="dt">DateTime</param>
        /// <param name="startingMonth">int? : 年度の開始月</param>
        /// <returns>int</returns>
        public static int FiscalYear(this DateTime dt, int? startingMonth = null)
        {
            return (dt.Month >= (startingMonth ?? FiscalYearStartingMonth)) ? dt.Year : dt.Year - 1;
        }
        /// <summary>
        /// 勤務時間により、休憩開始時間から休憩終了時間を計算する
        /// </summary>
        /// <param name="kintai">Kintai</param>
        /// <returns>int</returns>
        public static string CalcKyukei(this Kintai kintai)
        {
            //勤務時間を算出
            var str = DateTime.TryParse($"{kintai.StrTime.Substring(0, 2)}:{kintai.StrTime.Substring(2, 2)}",out _) ? DateTime.Parse($"{kintai.StrTime.Substring(0, 2)}:{kintai.StrTime.Substring(2, 2)}"): DateTime.Parse(kintai.StrTime);
            var end = DateTime.TryParse($"{kintai.EndTime.Substring(0, 2)}:{kintai.EndTime.Substring(2, 2)}", out _) ? DateTime.Parse($"{kintai.EndTime.Substring(0, 2)}:{kintai.EndTime.Substring(2, 2)}") : DateTime.Parse(kintai.EndTime);
            var kyustr = DateTime.TryParse($"{kintai.KyuStrTime.Substring(0, 2)}:{kintai.KyuStrTime.Substring(2, 2)}", out _) ? DateTime.Parse($"{kintai.KyuStrTime.Substring(0, 2)}:{kintai.KyuStrTime.Substring(2, 2)}") : DateTime.Parse(kintai.KyuStrTime);
            var zikan = end - str;
            if(zikan.Hours >= HOUTEI_TIME)
            {
                return kyustr.AddMinutes(60).ToString("t");
            }
            else
            {
                return kyustr.AddMinutes(45).ToString("t");
            }
        }
    }
}
