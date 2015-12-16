using System;
using System.Collections.Generic;
using System.Collections.Immutable;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Drawing;

namespace CoolEditor
{
    public class DisplayIntellisense
    {
        public DisplayIntellisense() { }

        private static GetIntellisense getIntellisense = new GetIntellisense();

        /// <summary>
        /// Show the information of the members of a namespace, Class, Structure, Enum...
        /// </summary>
        /// <param name="myTextBox"></param>
        public void ShowIntellisense(MyTextBox myTextBox)
        {
            if (myTextBox.IntellisenseBox.Items.Count > 0)
            {
                myTextBox.IntellisenseBox.Items.Clear();
            }

            // Get the entire program text
            string code = myTextBox.Text;

            // Get the member information of the last word user has typed
            List<IntellisenseItem> itemList = new List<IntellisenseItem>();
            itemList = getIntellisense.GetMembers(code,myTextBox.SelectionStart);

            // Add these items to the Intellisense box
            foreach (IntellisenseItem item in itemList)
            {
                myTextBox.IntellisenseBox.AddItem(item);
            }
            ShowIntellisenseBox(myTextBox);
        }

        /// <summary>
        /// Show accessible local variables at the scope 
        /// </summary>
        /// <param name="myTextBox"></param>
        public void ShowLocalVariables(MyTextBox myTextBox)
        {
            if (myTextBox.IntellisenseBox.Items.Count > 0)
            {
                myTextBox.IntellisenseBox.Items.Clear();
            }
            string code = myTextBox.Text;
            List<IntellisenseItem> itemList = new List<IntellisenseItem>();
            itemList = getIntellisense.GetLocalVariables(code, myTextBox.SelectionStart);
            foreach (IntellisenseItem item in itemList)
            {
                myTextBox.IntellisenseBox.AddItem(item);
            }
            ShowIntellisenseBox(myTextBox);
        }


        /// <summary>
        /// Show the information of the methods, which are members of a Class, Structure...
        /// </summary>
        /// <param name="myTextBox"></param>
        public void ShowMethodInfo(MyTextBox myTextBox)
        {
            IntellisenseItem item = (IntellisenseItem)myTextBox.IntellisenseBox.SelectedItem;
            HideIntellisenseBox(myTextBox);
            if (myTextBox.IntellisenseBox.Items.Count > 0)
            {
                myTextBox.IntellisenseBox.Items.Clear();
            }
            List<IntellisenseItem> itemList = new List<IntellisenseItem>();
            itemList = getIntellisense.GetMethodInfo(item);
            foreach (IntellisenseItem newItem in itemList)
            {
                IntellisenseItem methodInfo = new IntellisenseItem();
                methodInfo.Text = newItem.Text;
                myTextBox.IntellisenseBox.AddItem(methodInfo);
            }
            ShowIntellisenseBox(myTextBox);
        }

        /// <summary>
        /// Show Intellisense box
        /// </summary>
        /// <param name="myTextBox"></param>
        public void ShowIntellisenseBox(MyTextBox myTextBox)
        {

            if (myTextBox.IntellisenseBox.Items.Count > 0)
            {
                myTextBox.IntellisenseBox.SelectedIndex = 0;
                Point topLeft = myTextBox.GetPositionFromCharIndex(myTextBox.SelectionStart);
                topLeft.Offset(-35, 18);

                #region Place the intellisense box, to fix the space...
                if (myTextBox.Size.Height < (topLeft.Y + myTextBox.IntellisenseBox.Height))
                {
                    topLeft.Offset(0, -18 - 18 - myTextBox.IntellisenseBox.Height);
                }

                if (myTextBox.Size.Width < (topLeft.X + myTextBox.IntellisenseBox.Width))
                {
                    topLeft.Offset(35 + 15 - myTextBox.IntellisenseBox.Width, 0);
                }
                if (topLeft.X < 0)
                {
                    topLeft.X = 0;
                }
                if (topLeft.Y < 0)
                {
                    topLeft.Y = 0;
                }
                #endregion
                myTextBox.IntellisenseBox.Location = topLeft;
                myTextBox.IntellisenseBox.Visible = true;
                myTextBox.Focus();
            }
            else
            {
                myTextBox.IntellisenseBox.Visible = false;
            }
        }


        /// <summary>
        /// Hides the intellisense box.
        /// </summary>
        public void HideIntellisenseBox(MyTextBox myTextBox)
        {
            myTextBox.IntellisenseBox.Items.Clear();
            myTextBox.IntellisenseBox.Visible = false;
        }

        /// <summary>
        /// Navigates up in the intellisense box.
        /// </summary>
        public void NavigateUp(MyTextBox myTextBox, int step)
        {
            #region Some checkings for the intellisense box
            //Do nothing if the intellisense is not visible...
            if (myTextBox.IntellisenseBox.Visible)
            {
                return;
            }
            //If our box has no elements, do not show it...
            if (myTextBox.IntellisenseBox.Items.Count <= 0)
            {
                return;
            }
            #endregion

            // Navigate down the Intellisense box according to the step
            if (myTextBox.IntellisenseBox.SelectedIndex > 0)
            {
                myTextBox.IntellisenseBox.SelectedIndex -= step;
            }
            else
            {
                myTextBox.IntellisenseBox.SelectedIndex = 0;
            }
        }

        /// <summary>
        /// Navigates down in the intellisense box.
        /// </summary>
        public void NavigateDown(MyTextBox myTextBox, int step)
        {
            #region Some checkings for the intellisense box

            //Do nothing if the intellisense is not visible...
            if (!myTextBox.IntellisenseBox.Visible)
            {
                return;
            }

            //If our box has no elements, do not show it...
            if (myTextBox.IntellisenseBox.Items.Count <= 0)
            {
                return;
            }
            #endregion

            // Navigate down the Intellisense box according to the step
            if (myTextBox.IntellisenseBox.SelectedIndex < myTextBox.IntellisenseBox.Items.Count - 1)
            {
                myTextBox.IntellisenseBox.SelectedIndex += step;
            }
            else
            {
                myTextBox.IntellisenseBox.SelectedIndex = myTextBox.IntellisenseBox.Items.Count - 1;
            }
        }

        /// <summary>
        /// Navigates to the first element in the intellisense box.
        /// </summary>
        public void NavigateHome(MyTextBox myTextBox)
        {
            #region Some checkings for the intellisense box
            //Do nothing if the intellisense is not visible...
            if (!myTextBox.IntellisenseBox.Visible)
            {
                return;
            }
            //If our box has no elements, do not show it...
            if (myTextBox.IntellisenseBox.Items.Count <= 0)
            {
                return;
            }
            #endregion

            myTextBox.IntellisenseBox.SelectedIndex = 0;
        }

        /// <summary>
        /// Navigates to the last element in the intellisense box.
        /// </summary>
        public void NavigateEnd(MyTextBox myTextBox)
        {
            #region Some checkings for the intellisense box
            //Do nothing if the intellisense is not visible...
            if (!myTextBox.IntellisenseBox.Visible)
            {
                return;
            }
            //If our box has no elements, do not show it...
            if (myTextBox.IntellisenseBox.Items.Count <= 0)
            {
                return;
            }
            #endregion

            myTextBox.IntellisenseBox.SelectedIndex = myTextBox.IntellisenseBox.Items.Count - 1;
        }

        /// <summary>
        /// Confirms the selection from the intellisense, and write the selected text back to the textbox.
        /// </summary>
        public void ConfirmIntellisense(MyTextBox myTextBox)
        {
            IntellisenseItem item = (IntellisenseItem)myTextBox.IntellisenseBox.SelectedItem;
            if (item.Tag != null)
            {
                string wordSelected = item.Text;

                //Get the actual position
                int currentPosition = myTextBox.SelectionStart;

                //Get the start position of the last word
                //Set position to write back the selected word 
                string code = myTextBox.Text.Substring(0, myTextBox.SelectionStart);
                int pos = code.Length - 1;

                while (pos >= 1)
                {
                    string substr = code.Substring(pos, 1);
                    if (substr[0] == '.')
                    {
                        break;
                    }
                    else
                    {
                        pos--;

                    }
                }

                myTextBox.SelectionStart = pos + 1;
                myTextBox.SelectionLength = currentPosition - pos - 1;
                myTextBox.SelectedText = "";


                ////Change the word
                myTextBox.SelectionLength = 0;
                myTextBox.SelectedText = wordSelected;

                //Hide the intellisense box
                if (item.MethodInfoList.Count > 0)
                {
                    ShowMethodInfo(myTextBox);
                }
                else
                {
                    HideIntellisenseBox(myTextBox);
                }
            }
        }

    }
}
