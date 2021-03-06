﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Main_Project
{
    public partial class AppointmentUserControl : UserControl
    {
        //holds the value of the data selected in the datagridview
        private static string appointmentID;
        private static string staffName;
        private static string patientName;


        static private bool valueNotSelected = false;

        //singleton initiation
        private static AppointmentUserControl _instance;


        public static AppointmentUserControl Instance
        {
            get
            {
                //check if any previous instance was properly disposed
                if (_instance == null || _instance.IsDisposed == true)
                    _instance = new AppointmentUserControl();
                return _instance;
            }
        }

        //public strings
        public static string AppointmentID
        {
            get
            {
                return appointmentID;
            }

            set
            {
                appointmentID = value;
            }
        }

        public static string StaffName
        {
            get
            {
                return staffName;
            }

            set
            {
                staffName = value;
            }
        }

        public static string PatientName
        {
            get
            {
                return patientName;
            }

            set
            {
                patientName = value;
            }
        }

        public AppointmentUserControl()
        {
            InitializeComponent();
        }


        /// <summary>
        /// Method that removes any user control present in front of the appointment user control
        /// </summary>
        public static void RemoveBook()
        {
            Instance.Controls.Remove(BookAppointUserControl.Instance);
            BookAppointUserControl.Instance.Dispose();
            Instance.Controls.Remove(EditAppoinUserControl.Instance);
            EditAppoinUserControl.Instance.Dispose();

        }

        /// <summary>
        /// Load method for the appointment user control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void AppointmentUserControl_Load(object sender, EventArgs e)
        {
            // TODO: This line of code loads data into the 'oversurgeryDataSet.userdata' table. You can move, or remove it, as needed.
            this.userdataTableAdapter.Fill(this.oversurgeryDataSet.userdata);

            DataSet dsAppoint = DBConnection.getDBConnectionInstance().getDataSet(Constants.selectAllAppointment);

            //get the table to be displayed from the data set
            DataTable dtAppoint = dsAppoint.Tables[0];

            //set the data source for the data grid view
            dataGridView1.DataSource = dtAppoint;
        }

        //Button Methods


        /// <summary>
        /// Method that cancels/deletes a selected appointment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnCancel_Click(object sender, EventArgs e)
        {
            Logger.Instance.Log("AppointmentUserControl:btnCancel_Click() -> Canceling an appointment.");

            //check to see if an appointment is seleted
            if (valueNotSelected == true)
            {
                MessageBox.Show("Please select the appointment you want to cancel.");
            }
            else
            {
                try
                {
                    //warning message to prevent the user from deleting an appointment by mistake.
                    DialogResult dr = MessageBox.Show("Do you really want to delete this appointment?",
                      "Warning", MessageBoxButtons.YesNo);
                    switch (dr)
                    {
                        case DialogResult.Yes:
                            //try to delete appointment
                            DBConnection.getDBConnectionInstance().SqlStatementExecute(Constants.DeleteAppointment(AppointmentID));
                            //try to delete the shift related to the same appointment
                            DBConnection.getDBConnectionInstance().SqlStatementExecute(Constants.DeleteShift(AppointmentID));
                            MessageBox.Show("Appointment deleted successfully!");

                            break;
                        case DialogResult.No:
                            break;
                    }

                }
                catch (Exception ex)
                {
                    MessageBox.Show(ex.ToString());
                }
            }
        }

        /// <summary>
        /// Method that opens the edit appointment windows according to the selected appointment
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnEdit_Click(object sender, EventArgs e)
        {
            //check to see if an appointment is selected
            if (valueNotSelected == true)
            {
                MessageBox.Show("Please select the appointment you want to edit.");
            }
            else
            {
                Instance.Controls.Add(EditAppoinUserControl.Instance);

                EditAppoinUserControl.Instance.Dock = DockStyle.Fill;
                EditAppoinUserControl.Instance.BringToFront();
            }
        }

        /// <summary>
        /// Method that runs the book appointment user control
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void btnBook_Click(object sender, EventArgs e)
        {
            Instance.Controls.Add(BookAppointUserControl.Instance);


            BookAppointUserControl.Instance.Dock = DockStyle.Fill;
            BookAppointUserControl.Instance.BringToFront();

        }

        //Methods


        /// <summary>
        /// Gets the value from the selected row in the data grid view
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void dataGridView1_CellClick(object sender, DataGridViewCellEventArgs e)
        {
            var index = e.RowIndex;
            valueNotSelected = false;

            DataGridViewRow selectedRow = dataGridView1.Rows[index];
            try
            {
                AppointmentID = selectedRow.Cells[0].Value.ToString();

                PatientName = selectedRow.Cells[5].Value.ToString();
                StaffName = selectedRow.Cells[4].Value.ToString();

                int appointId = Convert.ToInt32(AppointmentID);
                Console.WriteLine(AppointmentID);
            }
            catch (Exception ex)
            {
                valueNotSelected = true;
            }
        }

        /// <summary>
        /// Method that return the appointmentID public value
        /// </summary>
        /// <returns></returns>
        public static string ReturnAppointmentValue()
        {
            return AppointmentID;
        }

        public static string ReturnStaffValue()
        {
            return StaffName;
        }
        public static string ReturnPatientValue()
        {
            return PatientName;
        }

    }
}
