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
using Newtonsoft.Json;


namespace StudentScoreAVL
{
    public partial class MainForm : Form
    {
        
        string DataFilePath = "StudentsPerformance.csv";
        AVLTree CAYKHONGTRUNG;
        AVLTree CAYBITRUNG;
        AVLTree avlTree = new AVLTree();
        AVLTree tempTree = null;
        List<Student> students = new List<Student>();
        Student currentStudent = null;
        public MainForm()
        {
            InitializeComponent();
        }
        private void LoadCSVData()
        {
            students =new List<Student>();
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
            BuildAVLFromList();
            dataGridView1.DataSource = students;
        }
        private void BuildAVLFromList()
        {
            avlTree = new AVLTree();
            foreach (var s in students)
                avlTree.Insert(s);
        }
        private void button1_Click(object sender, EventArgs e)
        {
            LoadCSVData();
            dataGridView1.Visible = true;
            
        }
        //Lưu
        private void SaveToCSV(string path)
        {
            using (StreamWriter w = new StreamWriter(path))
            {
                w.WriteLine("ID,Gender,RaceEthnicity,ParentalEducation,Lunch,TestPreparationCourse,MathScore,ReadingScore,WritingScore,Van");
                foreach (var s in students)
                {
                    w.WriteLine($"{s.ID},{s.Gender},{s.RaceEthnicity},{s.ParentalEducation},{s.Lunch},{s.TestPreparationCourse},{s.MathScore},{s.ReadingScore},{s.WritingScore},{s.Van}");
                }
            }
            MessageBox.Show("Lưu thành công", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void LuuJson(AVLTree tree, string fileName)
        {
            if (tree == null || tree.Root == null)
            {
                MessageBox.Show("Cây rỗng, không có dữ liệu để xuất!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            var data = tree.InOrderTraversal();

            SaveFileDialog sfd = new SaveFileDialog();
            sfd.Filter = "JSON files (*.json)|*.json";
            sfd.FileName = fileName;

            if (sfd.ShowDialog() == DialogResult.OK)
            {
                string json = JsonConvert.SerializeObject(
                    data,
                    Formatting.Indented
                );

                File.WriteAllText(sfd.FileName, json, Encoding.UTF8);

                MessageBox.Show("Xuất JSON thành công!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Information);
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
                SaveToCSV(filePath);
                MessageBox.Show("Đã lưu file CSV");
            }
        }
        //Tìm kiếm
        private void btnSearch_Click(object sender, EventArgs e)
        {
            string nhap = txtSearch.Text;
            if (string.IsNullOrEmpty(nhap))
            {
                MessageBox.Show("Vui lòng nhập ID học sinh để tìm!", "Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Warning);
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
                MessageBox.Show("Không tìm thấy học sinh có ID này!", "Kết quả",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            dataGridView1.DataSource = new List<Student> { student };
            
        }
        //Thêm
        private void btnAdd_Click(object sender, EventArgs e)
        {
            try
            {
                if (string.IsNullOrWhiteSpace(txtSearch.Text))
                {
                    MessageBox.Show("Vui lòng nhập ID học sinh!","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Error);
                    return;
                }
                if (!int.TryParse(txtSearch.Text, out int newID))
                {
                    MessageBox.Show("ID phải là số!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (avlTree.Search(newID) != null)
                {
                    MessageBox.Show("ID trùng.","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Error);
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
                    MessageBox.Show("Vui lòng nhập đầy đủ thông tin học sinh!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                if (!double.TryParse(txtMath.Text, out double math) ||
                    !double.TryParse(txtReading.Text, out double reading) ||
                    !double.TryParse(txtWriting.Text, out double writing))
                {
                    MessageBox.Show("Điểm phải là số!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                    return;
                }
                Student sd = new Student
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
                students.Add(sd);
                BuildAVLFromList();
                dataGridView1.DataSource = students.ToList();
                MessageBox.Show("Thêm học sinh thành công!","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show($"Lỗi khi thêm học sinh: {ex.Message}","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Error);
            }
        }
        //Xóa
        private void btnDelete_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrWhiteSpace(txtSearch.Text))
            {
                MessageBox.Show("Vui lòng nhập ID học sinh cần xóa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }

            if (!int.TryParse(txtSearch.Text.Trim(), out int id))
            {
                MessageBox.Show("ID phải là số nguyên!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            var student = students.FirstOrDefault(s => s.ID == id);
            if (student == null)
            {
                MessageBox.Show("Không tìm thấy học sinh có ID này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                students.Remove(student);
                BuildAVLFromList();
                dataGridView1.DataSource = null;
                dataGridView1.DataSource = students.ToList();
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
                MessageBox.Show("Vui lòng nhập ID sinh viên cần sửa!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(txtSearch.Text.Trim(), out int id))
            {
                MessageBox.Show("ID phải là số!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!double.TryParse(txtMath.Text, out double math) ||
                !double.TryParse(txtReading.Text, out double reading) ||
                !double.TryParse(txtWriting.Text, out double writing))
            {
                MessageBox.Show("Điểm phải là số hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Không tìm thấy sinh viên có ID này!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            BuildAVLFromList();
            dataGridView1.DataSource = students.ToList();
            MessageBox.Show("Cập nhật thành công!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        private void btnXuatTang_Click(object sender, EventArgs e)
        {
            if (!int.TryParse(txtXuatTang.Text.Trim(), out int level))
            {
                MessageBox.Show("Vui lòng nhập tầng hợp lệ (số nguyên >= 0)!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            BuildAVLFromList(); 
            var nodes = avlTree.XepTang(level);
            if (nodes.Count == 0)
            {
                MessageBox.Show($"Không có node nào ở tầng {level}!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
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
                MessageBox.Show("Vui lòng nhập số dòng hợp lệ!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (!int.TryParse(RangeDongCuoi.Text.Trim(), out int enddong) || enddong <= 0 || enddong > dataGridView1.Rows.Count)
            {
                MessageBox.Show("Vui lòng nhập Dòng Kết Thúc hợp lệ (số nguyên > 0)!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dataGridView1.Rows.Count == 0)
            {
                MessageBox.Show("Không có dữ liệu trong bảng chính!", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            List<Student> selectedStudents = new List<Student>();
            int batdau = dongbatdau - 1;
            if (batdau > dataGridView1.Rows.Count || batdau < 0)
            {
                MessageBox.Show("Chỉ số không hợp lệ.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            if (dongbatdau > enddong)
            {
                MessageBox.Show("Dòng bắt đầu không được lớn hơn dòng kết thúc.", "Thông báo", MessageBoxButtons.OK, MessageBoxIcon.Error);
                return;
            }
            for (int i = batdau; i < enddong; i++)
            {
                if (dataGridView1.Rows[i].DataBoundItem is Student student)
                {
                    selectedStudents.Add(student);
                }
            }
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = selectedStudents;
            
            dataGridView1.Visible = true;
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
                MessageBox.Show("Chưa có cây AVL","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Warning);
                return;
            }

            if (!int.TryParse(txtXuat.Text, out int tangCanXem) || tangCanXem < 0)
            {
                MessageBox.Show("Vui lòng nhập số tầng hợp lệ (số nguyên >= 0)!","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }
            var ketQua = treeToUse.XepTang(tangCanXem);
            if (ketQua.Count == 0)
            {
                MessageBox.Show($"Không có node nào ở tầng {tangCanXem}","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Error);
                dataGridView1.DataSource = null;
                return;
            }
            dataGridView1.Visible = true;
       
            dataGridView1.DataSource = ketQua.Select(s => new
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
        //Sắp xếp theo Key
        private void cbbSapXep_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (cbbSapXep.SelectedItem == null) return;
            string key = cbbSapXep.SelectedItem.ToString();
            Func<Student, double> keySelector = null;
            switch (key)
            {
                case "ID":
                    keySelector = s => s.ID;
                    break;
                case "MathScore":
                    keySelector = s => s.MathScore;
                    break;
                case "ReadingScore":
                    keySelector = s => s.ReadingScore;
                    break;
                case "WritingScore":
                    keySelector = s => s.WritingScore;
                    break;
                case "Điểm TB":
                    keySelector = s => s.TB;
                    break;
            }
            if (keySelector == null) return;
            avlTree.XayCay(keySelector, out CAYKHONGTRUNG, out CAYBITRUNG);
            LoadCayDangChon();
        }
        private void LoadCayDangChon()
        {
            if (CAYKHONGTRUNG == null || CAYBITRUNG == null)
                return;

            AVLTree cayHienThi = radKhongTrung.Checked ? CAYKHONGTRUNG : CAYBITRUNG;

            string key = cbbSapXep.SelectedItem?.ToString();

            var data = cayHienThi.InOrderTraversal();
            switch (key)
            {
                case "ID": data = data.OrderBy(s => s.ID).ToList(); break;
                case "MathScore": data = data.OrderBy(s => s.MathScore).ToList(); break;
                case "ReadingScore": data = data.OrderBy(s => s.ReadingScore).ToList(); break;
                case "WritingScore": data = data.OrderBy(s => s.WritingScore).ToList(); break;
                case "Điểm TB": data = data.OrderBy(s => s.TB).ToList(); break;
            }
            dataGridView1.DataSource = data;
        }

        private void radKhongTrung_CheckedChanged(object sender, EventArgs e)
        {
            if (radKhongTrung.Checked)
                LoadCayDangChon();
        }

        private void radTrung_CheckedChanged(object sender, EventArgs e)
        {
            if (radTrung.Checked)
                LoadCayDangChon();
        }

        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0) return;

            Student hs = dataGridView1.Rows[e.RowIndex].DataBoundItem as Student;
            if (hs == null) return;

            txtSearch.Text = hs.ID.ToString();
            CbbGender.Text = hs.Gender;
            txtRace.Text = hs.RaceEthnicity;
            txtEducation.Text = hs.ParentalEducation;
            txtLunch.Text = hs.Lunch;
            txtTest.Text = hs.TestPreparationCourse;
            txtMath.Text = hs.MathScore.ToString();
            txtReading.Text = hs.ReadingScore.ToString();
            txtWriting.Text = hs.WritingScore.ToString();
        }

        private void btnThongKeNodeTrung_Click(object sender, EventArgs e)
        {
            if (cbbSapXep.SelectedItem == null)
            {
                MessageBox.Show("Chưa chọn key để thống kê!","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Error);
                return;
            }

            string key = cbbSapXep.SelectedItem.ToString();
            Func<Student, double> keySelector = null;

            switch (key)
            {
                case "ID":
                    keySelector = s => s.ID; 
                    break;
                case "MathScore": 
                    keySelector = s => s.MathScore; 
                    break;
                case "ReadingScore": 
                    keySelector = s => s.ReadingScore; 
                    break;
                case "WritingScore": 
                    keySelector = s => s.WritingScore; 
                    break;
                case "Điểm TB": 
                    keySelector = s => s.TB; 
                    break;
            }

            if (keySelector == null) return;

            var list = avlTree.InOrderTraversal()
                              .GroupBy(x => keySelector(x))
                              .Where(g => g.Count() > 1)//lấy trùng
                              .Select(g => new
                              {
                                  Key = g.Key,
                                  Count = g.Count()-1
                              }).OrderBy(x => x.Key)
                              .ToList();

            if (list.Count == 0)
            {
                MessageBox.Show("Không có giá trị bị trùng!");
                return;
            }
            dataGridView1.DataSource = list;
        }

        private void btnTrolai_Click(object sender, EventArgs e)
        {
            dataGridView1.DataSource = null;
            dataGridView1.DataSource = students.ToList();
            BuildAVLFromList();
        }

        private void btnKiemKe_Click(object sender, EventArgs e)
        {
            if (CAYKHONGTRUNG == null || CAYBITRUNG == null)
            {
                MessageBox.Show("Chưa tách cây theo key!", "Thông báo",MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            int soNodeKhongTrung = CAYKHONGTRUNG.InOrderTraversal().Count;
            int soNodeBiTrung = CAYBITRUNG.InOrderTraversal().Count;

            MessageBox.Show( $"Số node cây không trùng: {soNodeKhongTrung}\n" +$"Số node cây trùng: {soNodeBiTrung}","Kết quả thống kê node",MessageBoxButtons.OK);
        }

        private void btnItNhieu_Click(object sender, EventArgs e)
        {
            if (cbbSapXep.SelectedItem == null)
            {
                MessageBox.Show("Chưa chọn key!");
                return;
            }

            Func<Student, double> keySelector = null;
            string key = cbbSapXep.SelectedItem.ToString();

            switch (key)
            {
                case "ID":
                    keySelector = s => s.ID;
                    break;
                case "MathScore":
                    keySelector = s => s.MathScore;
                    break;
                case "ReadingScore":
                    keySelector = s => s.ReadingScore;
                    break;
                case "WritingScore":
                    keySelector = s => s.WritingScore;
                    break;
                case "Điểm TB":
                    keySelector = s => s.TB;
                    break;
                default:
                    keySelector = null;
                    break;
            }

            if (keySelector == null) return;

            var dup = avlTree.InOrderTraversal()
                .GroupBy(x => keySelector(x))
                .Select(g => new { Key = g.Key, Count = g.Count() - 1 })
                .Where(x => x.Count > 0)
                .OrderBy(x => x.Count).ToList();

            if (dup.Count == 0) { MessageBox.Show("Không có node trùng!"); return; }
            if (dup.Count == 1)
            {
                var only = dup[0];
                MessageBox.Show(
                    $"Chỉ có 1 giá trị bị trùng:\n" +
                    $"Giá trị {only.Key} trùng {only.Count} lần\n" +
                    $"(Node trùng ít nhất: Không có)");
                return;
            }
            var min = dup.First();  // ít nhất
            var max = dup.Last();   // nhiều nhất
            MessageBox.Show(
                $"Node trùng nhiều nhất: {max.Key} → {max.Count} lần\n" +
                $"Node trùng ít nhất: {min.Key} → {min.Count} lần","Thông báo",MessageBoxButtons.OK,MessageBoxIcon.Information);
        }

        private void btnXuatJson_Click(object sender, EventArgs e)
        {
            if (CAYKHONGTRUNG == null || CAYBITRUNG == null)
            {
                MessageBox.Show("Chưa có dữ liệu cây. Vui lòng chọn key để tách cây trước!",
                                "Thông báo",
                                MessageBoxButtons.OK,
                                MessageBoxIcon.Warning);
                return;
            }

            DialogResult result = MessageBox.Show(
                "Bạn muốn xuất dữ liệu nào?\n\n" +
                "Yes  → Cây không trùng\n" +
                "No   → Cây bị trùng\n" +
                "Cancel → Cả hai cây",
                "Chọn loại cây cần xuất",
                MessageBoxButtons.YesNoCancel,
                MessageBoxIcon.Question
            );

            if (result == DialogResult.Yes)
            {
                LuuJson(CAYKHONGTRUNG, "Cay_Khong_Trung.json");
            }
            else if (result == DialogResult.No)
            {
                LuuJson(CAYBITRUNG, "Cay_Bi_Trung.json");
            }
            else if (result == DialogResult.Cancel)
            {
                LuuJson(CAYKHONGTRUNG, "Cay_Khong_Trung.json");
                LuuJson(CAYBITRUNG, "Cay_Bi_Trung.json");
            }
        }
    }
}
