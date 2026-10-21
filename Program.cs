using System;
using System.Drawing;
using System.Windows.Forms;

namespace SimpleCalculator
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new CalcForm());
        }
    }

    public class CalcForm : Form
    {
        private TextBox txtNum1;
        private TextBox txtNum2;
        private ComboBox cbOperator;
        private Button btnCalculate;
        private Label lblResult;

        public CalcForm()
        {
            this.Text = "간편 계산기";
            this.Size = new Size(320, 220);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;

            txtNum1 = new TextBox() { Location = new Point(20, 25), Width = 70, Text = "0" };
            cbOperator = new ComboBox() { Location = new Point(100, 24), Width = 50, DropDownStyle = ComboBoxStyle.DropDownList };
            cbOperator.Items.AddRange(new string[] { "+", "-", "×", "÷" });
            cbOperator.SelectedIndex = 0;

            txtNum2 = new TextBox() { Location = new Point(160, 25), Width = 70, Text = "0" };
            btnCalculate = new Button() { Text = "계산하기", Location = new Point(20, 65), Width = 210, Height = 35 };
            lblResult = new Label() { Text = "결과: ", Location = new Point(20, 115), Width = 250, Font = new Font("맑은 고딕", 11, FontStyle.Bold) };

            btnCalculate.Click += BtnCalculate_Click;

            this.Controls.Add(txtNum1);
            this.Controls.Add(cbOperator);
            this.Controls.Add(txtNum2);
            this.Controls.Add(btnCalculate);
            this.Controls.Add(lblResult);
        }

        private void BtnCalculate_Click(object sender, EventArgs e)
        {
            if (!double.TryParse(txtNum1.Text, out double n1) || !double.TryParse(txtNum2.Text, out double n2))
            {
                MessageBox.Show("올바른 숫자를 입력하세요.", "입력 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                return;
            }

            double result = 0;
            string op = cbOperator.SelectedItem.ToString();

            switch (op)
            {
                case "+": result = n1 + n2; break;
                case "-": result = n1 - n2; break;
                case "×": result = n1 * n2; break;
                case "÷":
                    if (n2 == 0)
                    {
                        MessageBox.Show("0으로 나눌 수 없습니다.", "계산 오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return;
                    }
                    result = n1 / n2;
                    break;
            }

            lblResult.Text = $"결과: {result:N2}";
        }
    }
}
