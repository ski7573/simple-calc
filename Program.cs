using System;
using System.Drawing;
using System.Windows.Forms;

namespace StandardCalculator
{
    static class Program
    {
        [STAThread]
        static void Main()
        {
            ApplicationConfiguration.Initialize();
            Application.Run(new CalculatorForm());
        }
    }

    public class CalculatorForm : Form
    {
        private TextBox txtDisplay;
        private Label lblFormula;

        private double firstNumber = 0;
        private string currentOperator = "";
        private bool isNewEntry = true;

        public CalculatorForm()
        {
            // 폼 기본 설정
            this.Text = "계산기";
            this.Size = new Size(340, 460);
            this.FormBorderStyle = FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.StartPosition = FormStartPosition.CenterScreen;
            this.BackColor = Color.FromArgb(243, 243, 243);

            // 이전 계산 수식 미리보기 라벨
            lblFormula = new Label()
            {
                Text = "",
                Location = new Point(16, 12),
                Size = new Size(292, 20),
                TextAlign = ContentAlignment.MiddleRight,
                ForeColor = Color.Gray,
                Font = new Font("맑은 고딕", 9)
            };

            // 숫자 디스플레이 화면
            txtDisplay = new TextBox()
            {
                Text = "0",
                Location = new Point(16, 35),
                Size = new Size(292, 45),
                ReadOnly = true,
                TextAlign = HorizontalAlignment.Right,
                Font = new Font("맑은 고딕", 22, FontStyle.Bold),
                BackColor = Color.White,
                BorderStyle = BorderStyle.FixedSingle
            };

            this.Controls.Add(lblFormula);
            this.Controls.Add(txtDisplay);

            CreateButtons();
        }

        private void CreateButtons()
        {
            // 계산기 버튼 레이아웃 배치표 (5행 4열)
            string[,] buttons = new string[5, 4]
            {
                { "C", "←", "±", "÷" },
                { "7", "8", "9", "×" },
                { "4", "5", "6", "-" },
                { "1", "2", "3", "+" },
                { "0", ".", "",  "=" }
            };

            int startX = 16;
            int startY = 95;
            int btnWidth = 68;
            int btnHeight = 55;
            int gap = 7;

            for (int row = 0; row < 5; row++)
            {
                for (int col = 0; col < 4; col++)
                {
                    string text = buttons[row, col];
                    if (string.IsNullOrEmpty(text)) continue;

                    Button btn = new Button()
                    {
                        Text = text,
                        Size = new Size(btnWidth, btnHeight),
                        Location = new Point(startX + col * (btnWidth + gap), startY + row * (btnHeight + gap)),
                        Font = new Font("맑은 고딕", 12, FontStyle.Bold),
                        FlatStyle = FlatStyle.Flat,
                        Cursor = Cursors.Hand
                    };

                    btn.FlatAppearance.BorderSize = 1;
                    btn.FlatAppearance.BorderColor = Color.FromArgb(215, 215, 215);

                    // 버튼 성격별 색상 구분
                    if (char.IsDigit(text[0]))
                    {
                        btn.BackColor = Color.White;
                        btn.Click += NumberButton_Click;
                    }
                    else if (text == "=")
                    {
                        // '=' 버튼은 2칸 너비 차지
                        btn.Size = new Size(btnWidth * 2 + gap, btnHeight);
                        btn.BackColor = Color.FromArgb(0, 120, 215);
                        btn.ForeColor = Color.White;
                        btn.Click += EqualsButton_Click;
                    }
                    else if (text == "+" || text == "-" || text == "×" || text == "÷")
                    {
                        btn.BackColor = Color.FromArgb(235, 235, 235);
                        btn.Click += OperatorButton_Click;
                    }
                    else
                    {
                        btn.BackColor = Color.FromArgb(235, 235, 235);
                        switch (text)
                        {
                            case "C": btn.Click += ClearButton_Click; break;
                            case "←": btn.Click += BackspaceButton_Click; break;
                            case "±": btn.Click += SignChangeButton_Click; break;
                            case ".": btn.Click += DotButton_Click; break;
                        }
                    }

                    this.Controls.Add(btn);
                }
            }
        }

        // 숫자 (0~9) 입력
        private void NumberButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;
            if (isNewEntry || txtDisplay.Text == "0")
            {
                txtDisplay.Text = btn.Text;
                isNewEntry = false;
            }
            else
            {
                if (txtDisplay.Text.Length < 16)
                    txtDisplay.Text += btn.Text;
            }
        }

        // 소수점 (.)
        private void DotButton_Click(object sender, EventArgs e)
        {
            if (isNewEntry)
            {
                txtDisplay.Text = "0.";
                isNewEntry = false;
                return;
            }

            if (!txtDisplay.Text.Contains("."))
            {
                txtDisplay.Text += ".";
            }
        }

        // 사칙연산자 (+, -, ×, ÷)
        private void OperatorButton_Click(object sender, EventArgs e)
        {
            Button btn = (Button)sender;

            if (!isNewEntry && !string.IsNullOrEmpty(currentOperator))
            {
                CalculateIntermediate();
            }

            firstNumber = double.Parse(txtDisplay.Text);
            currentOperator = btn.Text;
            lblFormula.Text = $"{firstNumber} {currentOperator}";
            isNewEntry = true;
        }

        // 계산 실행 (=)
        private void EqualsButton_Click(object sender, EventArgs e)
        {
            if (string.IsNullOrEmpty(currentOperator)) return;

            double secondNumber = double.Parse(txtDisplay.Text);
            lblFormula.Text = $"{firstNumber} {currentOperator} {secondNumber} =";

            double result = ExecuteOperation(firstNumber, secondNumber, currentOperator);

            txtDisplay.Text = result.ToString("G15");
            firstNumber = result;
            currentOperator = "";
            isNewEntry = true;
        }

        private void CalculateIntermediate()
        {
            double secondNumber = double.Parse(txtDisplay.Text);
            firstNumber = ExecuteOperation(firstNumber, secondNumber, currentOperator);
            txtDisplay.Text = firstNumber.ToString("G15");
        }

        private double ExecuteOperation(double n1, double n2, string op)
        {
            switch (op)
            {
                case "+": return n1 + n2;
                case "-": return n1 - n2;
                case "×": return n1 * n2;
                case "÷":
                    if (n2 == 0)
                    {
                        MessageBox.Show("0으로 나눌 수 없습니다.", "오류", MessageBoxButtons.OK, MessageBoxIcon.Warning);
                        return 0;
                    }
                    return n1 / n2;
                default: return n2;
            }
        }

        // 백스페이스 (← 한 글자 지우기)
        private void BackspaceButton_Click(object sender, EventArgs e)
        {
            if (isNewEntry) return;

            if (txtDisplay.Text.Length > 1)
            {
                txtDisplay.Text = txtDisplay.Text.Substring(0, txtDisplay.Text.Length - 1);
            }
            else
            {
                txtDisplay.Text = "0";
                isNewEntry = true;
            }
        }

        // 초기화 (C)
        private void ClearButton_Click(object sender, EventArgs e)
        {
            txtDisplay.Text = "0";
            lblFormula.Text = "";
            firstNumber = 0;
            currentOperator = "";
            isNewEntry = true;
        }

        // 부호 반전 (±)
        private void SignChangeButton_Click(object sender, EventArgs e)
        {
            if (txtDisplay.Text == "0") return;

            if (txtDisplay.Text.StartsWith("-"))
                txtDisplay.Text = txtDisplay.Text.Substring(1);
            else
                txtDisplay.Text = "-" + txtDisplay.Text;
        }
    }
}
