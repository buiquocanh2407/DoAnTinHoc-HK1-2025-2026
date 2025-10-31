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
using System.Diagnostics;

namespace StudentScoreAVL
{
    public partial class MainForm : Form
    {
        AVLTree avlTree = new AVLTree();
        List<Student> students = new List<Student>();
        Student currentStudent = null;

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
            List<Student> students = new List<Student>();
            int id = 1;
            foreach (var line in File.ReadLines(filePath).Skip(1))
            {
                var values = line.Split(',');
                for (int i = 0; i < values.Length; i++)
                    values[i] = values[i].Trim().Trim('"');
                if (values.Length >= 8)
                {
                    double.TryParse(values[5], out double math);
                    double.TryParse(values[6], out double reading);
                    double.TryParse(values[7], out double writing);
                    var student = new Student
                    {
                        ID = id++,
                        Gender = values[0],
                        RaceEthnicity = values[1],
                        ParentalEducation = values[2],
                        Lunch = values[3],
                        TestPreparationCourse = values[4],
                        MathScore = math,
                        ReadingScore = reading,
                        WritingScore = writing
                    };
                    students.Add(student);
                    avlTree.Insert(student);
                }
            }
            dataGridView1.DataSource = avlTree.InOrderTraversal();
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
                // Đọc file CSV
                string[] lines = File.ReadAllLines(filePath);
                MessageBox.Show("Đọc CSV thành công! Có " + lines.Length + " dòng dữ liệu.",
                                "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
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
                //tiêu đề
                writer.WriteLine("ID,gender,race/ethnicity,parental level of education,lunch,test preparation course,math score,reading score,writing score");
                foreach (var s in students)
                {
                    writer.WriteLine($"{s.ID},{s.Gender},{s.RaceEthnicity},{s.ParentalEducation},{s.Lunch},{s.TestPreparationCourse},{s.MathScore},{s.ReadingScore},{s.WritingScore}");
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
        //Tìm kiếm
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string input = txtSearch.Text;
            if (string.IsNullOrEmpty(input))
            {
                MessageBox.Show("Vui lòng nhập ID học sinh để tìm!", "Thông báo");
                return;
            }
            if (!int.TryParse(input, out int id))
            {
                MessageBox.Show("Lỗi, ID phải là số nguyên!");
                return;
            }

            Student student = avlTree.Search(id);
            if (student == null)
            {
                MessageBox.Show("Không tìm thấy học sinh có ID này!", "Kết quả");
                return;
            }
            dataGridView1.DataSource = new List<Student> { student };
            CbbGender.SelectedIndex = CbbGender.FindStringExact(student.Gender);
            txtRace.Text = student.RaceEthnicity;
            txtEducation.Text = student.ParentalEducation;
            txtLunch.Text = student.Lunch;
            txtTest.Text = student.TestPreparationCourse;
            txtMath.Text = student.MathScore.ToString();
            txtReading.Text = student.ReadingScore.ToString();
            txtWriting.Text = student.WritingScore.ToString();
            currentStudent = student;

        }
        //Thêm
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                // Kiểm tra nhập đủ chưa
                if (CbbGender.SelectedItem == null ||
                    string.IsNullOrWhiteSpace(txtRace.Text) ||
                    string.IsNullOrWhiteSpace(txtEducation.Text) ||
                    string.IsNullOrWhiteSpace(txtLunch.Text) ||
                    string.IsNullOrWhiteSpace(txtTest.Text) ||
                    string.IsNullOrWhiteSpace(txtMath.Text) ||
                    string.IsNullOrWhiteSpace(txtReading.Text) ||
                    string.IsNullOrWhiteSpace(txtWriting.Text))
                {
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin học sinh!");
                    return;
                }
                if (!double.TryParse(txtMath.Text, out double math) ||
                    !double.TryParse(txtReading.Text, out double reading) ||
                    !double.TryParse(txtWriting.Text, out double writing))
                {
                    MessageBox.Show("Điểm phải là số!");
                    return;
                }
                int newID = 1;
                var students = avlTree.InOrderTraversal();
                if (students.Count > 0)
                    newID = students.Max(s => s.ID) + 1;
                Student newStudent = new Student
                {
                    ID = newID,
                    Gender = CbbGender.SelectedItem.ToString(),
                    RaceEthnicity = txtRace.Text,
                    ParentalEducation = txtEducation.Text,
                    Lunch = txtLunch.Text,
                    TestPreparationCourse = txtTest.Text,
                    MathScore = math,
                    ReadingScore = reading,
                    WritingScore = writing
                };
                avlTree.Insert(newStudent);
                dataGridView1.DataSource = avlTree.InOrderTraversal();

                MessageBox.Show("✅ Thêm học sinh thành công!");
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm học sinh: {ex.Message}");
            }
        }
        //Xóa
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(txtSearch.Text.Trim()))
            {
                MessageBox.Show("Vui lòng nhập ID học sinh cần xóa!");
                return;
            }
            if (!int.TryParse(txtSearch.Text.Trim(), out int id))
            {
                MessageBox.Show("ID phải là số nguyên!");
                return;
            }
            Student student = avlTree.Search(id);

            if (student == null)
            {
                MessageBox.Show("Không tìm thấy học sinh có ID này!");
                return;
            }
            var confirm = MessageBox.Show($"Bạn có chắc muốn xóa học sinh ID {id} không?",
                                          "Xác nhận xóa",
                                          MessageBoxButtons.YesNo,
                                          MessageBoxIcon.Question);
            if (confirm == DialogResult.Yes)
            {
                avlTree.Delete(id);
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = avlTree.InOrderTraversal();
                MessageBox.Show("Đã xóa học sinh thành công!");
            }
        }
        //Số lượng các node
        private void btnThongKe_Click(object sender, EventArgs e)
        {
            var (leaf, one, two) = avlTree.CountNodeTypes();
            MessageBox.Show(
                $"Thống kê nút trong cây:\n" +
                $"- Nút lá (0 con): {leaf}\n" +
                $"- Nút có 1 con: {one}\n" +
                $"- Nút có 2 con: {two}",
                "Kết quả thống kê"
            );
        }
        //Sửa
        private void btnUpdate_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                MessageBox.Show("Vui lòng nhập ID sinh viên cần sửa!");
                return;
            }
            Student updatedStudent = new Student
            {
                ID = int.Parse(txtSearch.Text),
                Gender = CbbGender.Text,
                RaceEthnicity = txtRace.Text,
                ParentalEducation = txtEducation.Text,
                Lunch = txtLunch.Text,
                TestPreparationCourse = txtTest.Text,
                MathScore = double.Parse(txtMath.Text),
                ReadingScore = double.Parse(txtReading.Text),
                WritingScore = double.Parse(txtWriting.Text)
            };
            bool result = avlTree.Sua(updatedStudent);
            if (result)
            {
                MessageBox.Show("Cập nhật thành công!");
                LoadCSVData();
            }
            else
            {
                MessageBox.Show("Không tìm thấy sinh viên có ID này!");
            }
        }
    }
}
