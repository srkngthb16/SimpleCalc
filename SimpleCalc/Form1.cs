using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace SimpleCalc
{
    public partial class Form1 : Form
    {
        // The TextBox that receives digits (the one the user clicked last)
        private TextBox activeTextBox;

        // The selected operation: "+", "-", "*" or "/"
        private string selectedOperator = "";

        // Colors used by the result panel (normal state and error state)
        private readonly Color normalBack = Color.FromArgb(234, 243, 251);
        private readonly Color normalText = Color.FromArgb(31, 78, 121);
        private readonly Color errorBack = Color.FromArgb(252, 228, 236);
        private readonly Color errorText = Color.FromArgb(220, 53, 69);

        public Form1()
        {
            InitializeComponent();

            // The first number box is active when the program starts
            activeTextBox = txtFirstNumber;
            ResetDisplay();
        }

        // ---------- Helper methods ----------

        // Returns the result and info areas to their initial state
        private void ResetDisplay()
        {
            pnlResult.BackColor = normalBack;
            lblResultIcon.Visible = false;
            lblResultValue.ForeColor = normalText;
            lblResultValue.Text = "";
            lblInfoMessage.Text = "Enter two numbers, choose an operation, then press =.";
        }

        // Shows a successful result in the result panel
        private void ShowResult(double result)
        {
            pnlResult.BackColor = normalBack;
            lblResultIcon.Visible = false;
            lblResultValue.ForeColor = normalText;

            // Convert the number to text and show it
            lblResultValue.Text = result.ToString();
            lblInfoMessage.Text = "Calculation completed successfully.";
        }

        // Shows an error both in the result panel and in a MessageBox
        private void ShowError(string shortMessage, string detailMessage, string boxTitle)
        {
            pnlResult.BackColor = errorBack;
            lblResultIcon.Visible = true;
            lblResultValue.ForeColor = errorText;
            lblResultValue.Text = shortMessage;
            lblInfoMessage.Text = detailMessage;

            MessageBox.Show(detailMessage, boxTitle,
                MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        // ---------- Event handlers ----------

        // Common Click event for the digit buttons (0-9)
        private void NumberButton_Click(object sender, EventArgs e)
        {
            // Find out which button was clicked
            Button clickedButton = (Button)sender;

            // Append the digit to the end of the active TextBox
            activeTextBox.Text += clickedButton.Text;
        }

        // Common Click event for the operator buttons (+, -, *, /)
        private void OperatorButton_Click(object sender, EventArgs e)
        {
            Button clickedButton = (Button)sender;

            // Store the selected operation and show it in the info area
            selectedOperator = clickedButton.Text;
            lblInfoMessage.Text = "Selected operation: " + selectedOperator;

            // Move to the second number box so the user can type it next
            txtSecondNumber.Focus();
        }

        // Click event for the decimal point button
        private void btnDecimal_Click(object sender, EventArgs e)
        {
            // Get the decimal separator used by this computer ("," or ".")
            string separator = Application.CurrentCulture.NumberFormat.NumberDecimalSeparator;

            // A number can contain only one decimal separator
            if (activeTextBox.Text.Contains(separator))
                return;

            // If the box is empty start with "0", then add the separator
            if (activeTextBox.Text == "")
                activeTextBox.Text = "0";

            activeTextBox.Text += separator;
        }

        // Click event for the equals button: validation + calculation
        private void btnEquals_Click(object sender, EventArgs e)
        {
            // 1) Check that an operation has been selected
            if (selectedOperator == "")
            {
                ShowError("No operation!",
                    "Please select an operation (+, -, *, /) first.",
                    "Missing Operation");
                return;
            }

            // 2) Check that both inputs are valid numbers
            if (!double.TryParse(txtFirstNumber.Text, out double number1) ||
                !double.TryParse(txtSecondNumber.Text, out double number2))
            {
                ShowError("Invalid input!",
                    "Please enter valid numbers in both fields.",
                    "Invalid Input");
                return;
            }

            // 3) Perform the selected operation
            double result = 0;
            switch (selectedOperator)
            {
                case "+":
                    result = number1 + number2;
                    break;
                case "-":
                    result = number1 - number2;
                    break;
                case "*":
                    result = number1 * number2;
                    break;
                case "/":
                    // Check for zero BEFORE dividing
                    if (number2 == 0)
                    {
                        ShowError("Cannot divide by zero!",
                            "Cannot divide by zero. Division by zero is not possible " +
                            "because it would result in infinity, which is not a valid " +
                            "numerical result.",
                            "Division by Zero");
                        return;
                    }
                    result = number1 / number2;
                    break;
            }

            // 4) Display the result
            ShowResult(result);
        }

        // Click event for the Clear button: resets everything
        private void btnClear_Click(object sender, EventArgs e)
        {
            txtFirstNumber.Clear();
            txtSecondNumber.Clear();
            selectedOperator = "";
            ResetDisplay();

            // Put the cursor back in the first number box
            txtFirstNumber.Focus();
        }

        // Enter event of both TextBoxes: remember which box is active
        private void TextBox_Enter(object sender, EventArgs e)
        {
            activeTextBox = (TextBox)sender;
        }
    }
}