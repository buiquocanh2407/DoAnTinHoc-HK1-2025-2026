using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace StudentScoreAVL
{
    public class Student
    {
        public int ID { get; set; }
        public string Gender { get; set; }
        public string RaceEthnicity { get; set; }
        public string ParentalEducation { get; set; }
        public string Lunch { get; set; }
        public string TestPreparationCourse { get; set; }
        public double MathScore { get; set; }
        public double ReadingScore { get; set; }
        public double WritingScore { get; set; }
        public double Van { get; set; }
        public double diemTB()
        {
            double dtb= (MathScore + ReadingScore + WritingScore) / 3;
            return Math.Round(dtb, 2);
        }
        public double TB
        {
            get { return diemTB(); }
        }
        public string LoaiHS()
        {
            double dtb = diemTB();
            if (dtb <= 100 && dtb >= 80)
                return "Giỏi";
            else if (dtb < 80 && dtb >= 50)
                return "Khá";
            else if (dtb < 50 && dtb >= 30)
                return "Trung Bình";
            else
                return "Yếu";
        }
        public string Loai
        {
            get { return LoaiHS(); }
        }
    }

}
