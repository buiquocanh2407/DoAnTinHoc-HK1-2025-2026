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
        string DataFilePath = "StudentsPerformance.csv";
        AVLTree avlTree = new AVLTree();
        AVLTree tempTree = null;
        List<Student> students = new List<Student>();
        Student currentStudent = null;
        public MainForm()
        {
            InitializeComponent();
            LoadCSVData();
            string csvFilePath = "StudentsPerformance.csv";
            var csvData = ReadCsvFile(csvFilePath); 
            foreach (var row in csvData)
            {
                Console.WriteLine(string.Join(",", row));
            }
        }
        private void LoadCSVData()
        {
            avlTree = new AVLTree();
            List<Student> students = new List<Student>();
            foreach (var line in File.ReadLines(DataFilePath).Skip(1))
            {
                var v = line.Split(',');
                students.Add(new Student
                {
                    ID = int.Parse(v[0]),
                    Gender = v[1],
                    RaceEthnicity = v[2],
                    ParentalEducation = v[3],
                    Lunch = v[4],
                    TestPreparationCourse = v[5],
                    MathScore = double.Parse(v[6]),
                    ReadingScore = double.Parse(v[7]),
                    WritingScore = double.Parse(v[8]),
                    Van = (v.Length > 9 && double.TryParse(v[9], out double tmpVan)) ? tmpVan : 0
                });
            }
            foreach (var s in students)
                avlTree.Insert(s);
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
        private void button1_Click(object sender, EventArgs e)
        {
            LoadCSVData();
            dataGridView1.Visible = true;
            DGV2.Visible = false;
        }
        //Lưu
            private void SaveToCSV()
            {
                var list = avlTree.InOrderTraversal();

                using (StreamWriter w = new StreamWriter(DataFilePath))
                {
                    w.WriteLine("ID,Gender,RaceEthnicity,ParentalEducation,Lunch,TestPreparationCourse,MathScore,ReadingScore,WritingScore,Van");

                    foreach (var s in list)
                    {
                    w.WriteLine($"{s.ID},{s.Gender},{s.RaceEthnicity},{s.ParentalEducation},{s.Lunch},{s.TestPreparationCourse},{s.MathScore},{s.ReadingScore},{s.WritingScore},{s.Van}");
                    }
                }
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
                SaveToCSV();
                MessageBox.Show("Đã lưu file CSV!");
            }
        }
        //Tìm kiếm
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string nhap = txtSearch.Text;
            if (string.IsNullOrEmpty(nhap))
            {
                MessageBox.Show("Vui lòng nhập ID học sinh để tìm!", "Thông báo");
                return;
            }
            if (!int.TryParse(nhap, out int id))
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
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    MessageBox.Show("Vui lòng nhập ID học sinh!");
                    return;
                }
                if (!int.TryParse(txtSearch.Text, out int newID))
                {
                    MessageBox.Show("ID phải là số!");
                    return;
                }
                if (avlTree.Search(newID) != null)
                {
                    MessageBox.Show("ID trùng.");
                    return;
                }
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
                SaveToCSV();
                LoadCSVData();
                MessageBox.Show("✔️ Thêm học sinh thành công!");
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
            var xacnhan = MessageBox.Show(
                $"Bạn có chắc muốn xóa học sinh ID {id} không?",
                "Xác nhận xóa",
                MessageBoxButtons.YesNo,
                MessageBoxIcon.Question
            );
            if (xacnhan == DialogResult.Yes)
            {
                avlTree.Delete(id);
                SaveToCSV();
                LoadCSVData();
                MessageBox.Show("Đã xóa học sinh thành công!");
            }
        }
        //Số lượng các node
        private void btnThongKe_Click(object sender, EventArgs e)
        {
            var (la, motcon, haicon) = avlTree.CountNodeTypes();
            MessageBox.Show(
                "Thống kê nút trong cây:\n" +
                $"- Nút lá (0 con): {la}\n" +
                $"- Nút có 1 con: {motcon}\n" +
                $"- Nút có 2 con: {haicon}",
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
            if (!int.TryParse(txtSearch.Text.Trim(), out int id))
            {
                MessageBox.Show("ID phải là số!");
                return;
            }
            if (!double.TryParse(txtMath.Text, out double math) ||
                !double.TryParse(txtReading.Text, out double reading) ||
                !double.TryParse(txtWriting.Text, out double writing))
            {
                MessageBox.Show("Điểm phải là số hợp lệ!");
                return;
            }
            Student updated = new Student
            {
                ID = id,
                Gender = CbbGender.Text,
                RaceEthnicity = txtRace.Text,
                ParentalEducation = txtEducation.Text,
                Lunch = txtLunch.Text,
                TestPreparationCourse = txtTest.Text,
                MathScore = math,
                ReadingScore = reading,
                WritingScore = writing
            };
            bool ok = avlTree.Sua(updated);
            if (!ok)
            {
                MessageBox.Show("Không tìm thấy sinh viên có ID này!");
                return;
            }
            SaveToCSV();
            LoadCSVData();
            MessageBox.Show("Cập nhật thành công!");
        }
        private void btnXuatTang_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtXuatTang.Text.Trim(), out int level))
            {
                MessageBox.Show("Vui lòng nhập tầng hợp lệ (số nguyên >= 0)!");
                return;
            }
            var nodes = avlTree.XepTang(level);
            if (nodes.Count == 0)
            {
                MessageBox.Show($"Không có node nào ở tầng {level}!");
                return;
            }
            dataGridView1.DataSource = nodes;
        }
        private void btnThoat_Click(object sender, EventArgs e)
        {
            var xacnhan = MessageBox.Show("Bạn xác nhận thoát?","Xác nhận xóa", MessageBoxButtons.YesNo, MessageBoxIcon.Question);
            if (xacnhan == DialogResult.Yes)
                this.Close();
        }
        //Range dòng
        private void btnChonDong_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtRangeDongDau.Text.Trim(), out int dongbatdau) || dongbatdau <= 0)
            {
                MessageBox.Show("Vui lòng nhập số dòng hợp lệ!");
                return;
            }
            if (!int.TryParse(RangeDongCuoi.Text.Trim(), out int enddong) || enddong <= 0 || enddong > dataGridView1.Rows.Count)
            {
                MessageBox.Show("Vui lòng nhập Dòng Kết Thúc hợp lệ (số nguyên > 0)!");
                return;
            }
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu trong bảng chính!");
                return;
            }
            List<Student> selectedStudents = new List<Student>();
            int batdau = dongbatdau - 1;
            if (batdau > dataGridView1.Rows.Count || batdau < 0)
            {
                MessageBox.Show("Chỉ số không hợp lệ.");
                return;
            }
            if (dongbatdau > enddong)
            {
                MessageBox.Show("Dòng bắt đầu không được lớn hơn dòng kết thúc.");
                return;
            }
            for (int i = batdau; i < enddong; i++)
            {
                if (dataGridView1.Rows[i].DataBoundItem is Student student)
                {
                    selectedStudents.Add(student);
                }
            }
            DGV2.DataSource = null;
            DGV2.DataSource = selectedStudents;
            dataGridView1.Visible = false;
            DGV2.Visible = true;
            tempTree = new AVLTree();
            foreach (var s in selectedStudents)
            {
                tempTree.Insert(s);
            }
        }
        //Xuất tầng cây theo cây mới
        private void btnXuat_Click(object sender, EventArgs e)
        {
            AVLTree treeToUse = tempTree;

            if (treeToUse == null || treeToUse.Root == null)
            {
                MessageBox.Show("Chưa có cây AVL! Hãy tạo cây trước (bằng cách Range Dòng hoặc Load CSV).");
                return;
            }

            if (!int.TryParse(txtXuat.Text, out int tangCanXem) || tangCanXem < 0)
            {
                MessageBox.Show("Vui lòng nhập số tầng hợp lệ (số nguyên >= 0)!");
                return;
            }
            var ketQua = treeToUse.XepTang(tangCanXem);
            if (ketQua.Count == 0)
            {
                MessageBox.Show($"Không có node nào ở tầng {tangCanXem}");
                DGV2.DataSource = null;
                return;
            }
            dataGridView1.Visible = false;
            DGV2.Visible = true;
            DGV2.DataSource = ketQua.Select(s => new
            {
                s.ID,
                s.Gender,
                s.RaceEthnicity,
                s.ParentalEducation,
                s.Lunch,
                s.TestPreparationCourse,
                s.MathScore,
                s.ReadingScore,
                s.WritingScore,
                s.Van,
            }).ToList();
        }
    }
}
