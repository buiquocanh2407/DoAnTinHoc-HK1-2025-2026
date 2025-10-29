using System;
using System.IO;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace StudentScoreAVL
{
    public partial class MainForm : Form
    {
        AVLTree avlTree = new AVLTree();

        public MainForm()
        { 
            InitializeComponent();
            LoadCSVData();

            string csvFilePath = "StudentsPerformance.csv";
            var csvData = ReadCsvFile(csvFilePath);

            // In ra console để kiểm tra
            foreach (var row in csvData)
            {
                Console.WriteLine(string.Join(", ", row));
            }
        }
        private void LoadCSVData()
        {
            string filePath = "StudentsPerformance.csv";

            //list để chứa dữ liệu
            List<Student> students = new List<Student>();

            // Đọc từng dòng từ file CSV
            foreach (var line in File.ReadLines(filePath).Skip(1)) // Skip(1) bỏ dòng tiêu đề
            {
                var values = line.Split(',');
                for (int i = 0; i < values.Length; i++)
                {
                    values[i] = values[i].Trim().Trim('"');
                }


                if (values.Length >= 8)
                {
                    double math, reading, writing;

                    double.TryParse(values[5].Trim(), out math);
                    double.TryParse(values[6].Trim(), out reading);
                    double.TryParse(values[7].Trim(), out writing);

                    students.Add(new Student
                    {
                        Gender = values[0].Trim(),
                        RaceEthnicity = values[1].Trim(),
                        ParentalEducation = values[2].Trim(),
                        Lunch = values[3].Trim(),
                        TestPreparationCourse = values[4].Trim(),
                        MathScore = math,
                        ReadingScore = reading,
                        WritingScore = writing
                    });
                    avlTree.Insert(students.Last());

                }

            }

            // Hiển thị ra DataGridView
            dataGridView1.DataSource = students;


         
        }


        private List<string[]> ReadCsvFile(string filePath)
        {
            List<string[]> rows = new List<string[]>();

            try
            {
                string[] lines = File.ReadAllLines(filePath);

                foreach (string line in lines)
                {
                    string[] values = line.Split(',');
                    rows.Add(values);
                }

                MessageBox.Show("CSV file read successfully!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Error: {ex.Message}");
            }

            return rows;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void button1_Click(object sender, EventArgs e)
        {
            string filePath = "StudentsPerformance.csv"; 

            try
            {
                // Đọc toàn bộ dòng trong file CSV
                string[] lines = File.ReadAllLines(filePath);

                // Hiển thị thông báo số dòng đọc được
                MessageBox.Show("Đọc CSV thành công! Có " + lines.Length + " dòng dữ liệu.",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);

                // In thử 5 dòng đầu để test (xem trong Output Window)
                for (int i = 0; i < Math.Min(lines.Length, 5); i++)
                {
                    Console.WriteLine(lines[i]);
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Lỗi khi đọc file CSV: " + ex.Message,
                                "Lỗi", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
        }
        private void SaveToCSV(string filePath)
        {
            var students = avlTree.InOrderTraversal();

            if (students.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu để lưu!");
                return;
            }

            using (StreamWriter writer = new StreamWriter(filePath))
            {
                // Ghi dòng tiêu đề
                writer.WriteLine("gender,race/ethnicity,parental level of education,lunch,test preparation course,math score,reading score,writing score");

                // Ghi từng sinh viên
                foreach (var s in students)
                {
                    writer.WriteLine($"{s.Gender},{s.RaceEthnicity},{s.ParentalEducation},{s.Lunch},{s.TestPreparationCourse},{s.MathScore},{s.ReadingScore},{s.WritingScore}");
                }
            }
            MessageBox.Show("✅ Dữ liệu đã được ghi ra file CSV thành công!");
        }

        private void btnluu_Click(object sender, EventArgs e)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "CSV files (*.csv)|*.csv";
            saveFileDialog.Title = "Chọn nơi lưu file CSV";
            saveFileDialog.FileName = "StudentData_AVL.csv";

            if (saveFileDialog.ShowDialog() == DialogResult.OK)
            {
                string filePath = saveFileDialog.FileName;
                SaveToCSV(filePath);
            }
        }
    }
}
