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
        public MainForm()
        {
            InitializeComponent();

            string csvFilePath = "StudentsPerformance.csv"; // đường dẫn file CSV
            var csvData = ReadCsvFile(csvFilePath);

            // In ra console để kiểm tra
            foreach (var row in csvData)
            {
                Console.WriteLine(string.Join(", ", row));
            }
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
            string filePath = "StudentsPerformance.csv"; // nếu file nằm cùng chỗ với .exe

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
    }
}
