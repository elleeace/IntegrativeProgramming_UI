using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows;
using IntegrativeProgramming_UI.Helpers;

namespace IntegrativeProgramming_UI
{
    internal class FormBuilder
    {
        private static TextBlock CreateLabel(string text) => new TextBlock
        {
            Text = text,
            Foreground = Brushes.Gray,
            Margin = new Thickness(0, 10, 0, 5)
        };

        private static TextBox CreateTextBox(string name, string text = "") => new TextBox
        {
            Name = name,
            Text = text,
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1d1f26")),
            Foreground = Brushes.Gray,
            Padding = new Thickness(10),
            Margin = new Thickness(0, 0, 0, 15),
            BorderThickness = new Thickness(0)
        };


        private static ComboBox CreateComboBox(IEnumerable<string> items) => new ComboBox
        {
            ItemsSource = items.ToList(),
            Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1d1f26")),
            Foreground = Brushes.Gray,
            Padding = new Thickness(5),
            Margin = new Thickness(0, 0, 0, 15),
            BorderThickness = new Thickness(0)
        };

        private static ComboBox CreateComboBox(IEnumerable<string> items, string selectedItem)
        {
            var comboBox = new ComboBox
            {
                ItemsSource = items.ToList(),
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1d1f26")),
                Foreground = Brushes.Gray,
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 0, 15),
                BorderThickness = new Thickness(0)
            };

            if (!string.IsNullOrWhiteSpace(selectedItem) && comboBox.Items.Contains(selectedItem))
            {
                comboBox.SelectedItem = selectedItem;
            }

            return comboBox;
        }

        
        #region Create Form
        public static void ShowAddBookForm(StackPanel targetPanel, NorthvilleLibraryDataContext db, Action onSuccess)
        {
            targetPanel.Children.Clear();

            var fields = new Dictionary<string, TextBox>();
            string[] textLabels = { "Book Title", "Author", "ISBN", "Format", "Edition Number", "Publication Year", "Number of Copies" };

            foreach (var label in textLabels)
            {
                string name = $"txt{label.Replace(" ", "")}";
                targetPanel.Children.Add(CreateLabel(label));
                var textBox = CreateTextBox(name);
                fields[label] = textBox;
                targetPanel.Children.Add(textBox);
            }

            targetPanel.Children.Add(CreateLabel("Genre"));
            var cbGenre = CreateComboBox(db.book_genres.Select(g => g.genre_name));
            targetPanel.Children.Add(cbGenre);

            targetPanel.Children.Add(CreateLabel("Location"));
            var cbLocation = CreateComboBox(db.book_locations.Select(l => l.location_id));
            targetPanel.Children.Add(cbLocation);

            var btn = new Button
            {
                Content = "Add Book",
                Margin = new Thickness(0, 10, 0, 0)
            };

            btn.Click += (s, e) =>
            {
                string title = fields["Book Title"].Text;
                string author = fields["Author"].Text;
                string isbn = fields["ISBN"].Text;
                string format = fields["Format"].Text;
                string genreName = cbGenre.SelectedItem?.ToString();
                string locationId = cbLocation.SelectedItem?.ToString();

                if (string.IsNullOrWhiteSpace(title) || string.IsNullOrWhiteSpace(author) ||
                    string.IsNullOrWhiteSpace(isbn) || string.IsNullOrWhiteSpace(format) ||
                    string.IsNullOrWhiteSpace(genreName) || string.IsNullOrWhiteSpace(locationId))
                {
                    MessageBoxBuilder.ShowWarning("Please complete all fields.");
                    return;
                }

                if (!int.TryParse(fields["Edition Number"].Text, out int edition) ||
                    !int.TryParse(fields["Publication Year"].Text, out int year) ||
                    !int.TryParse(fields["Number of Copies"].Text, out int copies))
                {
                    MessageBoxBuilder.ShowWarning("Edition, Year, and Copies must be valid numbers.");
                    return;
                }

                var confirm = MessageBoxBuilder.ShowConfirm(
                    $"Are you sure you want to add {copies} book copy/copies?",
                    "Confirm Add");

                if (confirm != MessageBoxResult.Yes)
                    return;

                try
                {
                    string genreId = db.book_genres.First(g => g.genre_name == genreName).book_genre_id;

                    db.usp_AddNewBookWithCopies(
                        title, author, genreId.ToString(), format, isbn, edition, year, locationId, copies);

                    MessageBoxBuilder.ShowSuccess("Book and copies successfully added.");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBoxBuilder.ShowError("An error occurred while adding the book.\n\nDetails: " + ex.Message);
                }
            };


            targetPanel.Children.Add(btn);
        }

        public static void BuildAddCourseForm(StackPanel targetPanel, NorthvilleLibraryDataContext db, Action onSuccess)
        {
            targetPanel.Children.Clear();

            targetPanel.Children.Add(CreateLabel("Course ID"));
            var txtCourseId = CreateTextBox("txtCourseId");
            targetPanel.Children.Add(txtCourseId);

            targetPanel.Children.Add(CreateLabel("Course Name"));
            var txtCourseName = CreateTextBox("txtCourseName");
            targetPanel.Children.Add(txtCourseName);

            var btnSubmit = new Button
            {
                Content = "Add Course",
                Margin = new Thickness(0, 10, 0, 0)
            };

            btnSubmit.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtCourseId.Text) || string.IsNullOrWhiteSpace(txtCourseName.Text))
                {
                    MessageBoxBuilder.ShowWarning("Please fill in all fields.");
                    return;
                }

                try
                {
                    var newCourse = new course
                    {
                        course_id = txtCourseId.Text,
                        course_name = txtCourseName.Text
                    };

                    db.courses.InsertOnSubmit(newCourse);
                    db.SubmitChanges();

                    MessageBoxBuilder.ShowSuccess("Course successfully added.");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBoxBuilder.ShowError("An error occurred while adding the course.\n\nDetails: " + ex.Message);
                }
            };

            targetPanel.Children.Add(btnSubmit);
        }


        public static void BuildBorrowForm(StackPanel targetPanel, NorthvilleLibraryDataContext dbContext, Action onSuccess)
        {
            targetPanel.Children.Clear();

            var txtStudentId = CreateTextBox("txtStudentId");
            var txtBookCopyId = CreateTextBox("txtBookCopyId");

            targetPanel.Children.Add(CreateLabel("Student ID"));
            targetPanel.Children.Add(txtStudentId);
            targetPanel.Children.Add(CreateLabel("Book Copy ID"));
            targetPanel.Children.Add(txtBookCopyId);

            var btnSubmit = new Button
            {
                Content = "Borrow Book",
                Margin = new Thickness(0, 10, 0, 0)
            };

            btnSubmit.Click += (s, e) =>
            {
                string studentId = txtStudentId.Text;
                string bookCopyId = txtBookCopyId.Text;

                if (string.IsNullOrWhiteSpace(studentId) || string.IsNullOrWhiteSpace(bookCopyId))
                {
                    MessageBoxBuilder.ShowWarning("Please fill in both fields.");
                    return;
                }

                try
                {
                    dbContext.usp_BorrowBook(studentId, bookCopyId);
                    MessageBoxBuilder.ShowSuccess("Borrow successful!");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBoxBuilder.ShowError("An error occurred while borrowing the book.\n\nDetails: " + ex.Message);
                }
            };

            targetPanel.Children.Add(btnSubmit);
        }


        public static void BuildReturnForm(StackPanel targetPanel, NorthvilleLibraryDataContext dbContext, Action onSuccess)
        {
            targetPanel.Children.Clear();

            var txtBorrowId = CreateTextBox("txtBorrowId");
            var dpReturnDate = new DatePicker
            {
                Name = "dpReturnDate",
                SelectedDate = DateTime.Today,
                Margin = new Thickness(0, 0, 0, 10)
            };

            targetPanel.Children.Add(CreateLabel("Borrow ID"));
            targetPanel.Children.Add(txtBorrowId);
            targetPanel.Children.Add(CreateLabel("Return Date"));
            targetPanel.Children.Add(dpReturnDate);

            var btnSubmit = new Button
            {
                Content = "Return Book",
                Margin = new Thickness(0, 10, 0, 0)
            };

            btnSubmit.Click += (s, e) =>
            {
                string borrowId = txtBorrowId.Text;
                DateTime? returnDate = dpReturnDate.SelectedDate;

                if (string.IsNullOrWhiteSpace(borrowId) || returnDate == null)
                {
                    MessageBoxBuilder.ShowWarning("Please fill in all fields.");
                    return;
                }

                try
                {
                    dbContext.usp_ReturnBook(borrowId, returnDate.Value);
                    MessageBoxBuilder.ShowSuccess("Return recorded successfully.");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBoxBuilder.ShowError("An error occurred while recording the return.\n\nDetails: " + ex.Message);
                }
            };

            targetPanel.Children.Add(btnSubmit);
        }


        public static void BuildAttendanceForm(StackPanel targetPanel, NorthvilleLibraryDataContext dbContext, Action onSuccess)
        {
            targetPanel.Children.Clear();

            var txtAttendanceId = CreateTextBox("txtAttendanceId");
            var txtStudentId = CreateTextBox("txtStudentId");
            var dpDate = new DatePicker
            {
                SelectedDate = DateTime.Today,
                Margin = new Thickness(0, 0, 0, 10)
            };
            var txtTime = CreateTextBox("txtTimeOfVisit");
            txtTime.Text = DateTime.Now.ToString("HH:mm");

            targetPanel.Children.Add(CreateLabel("Attendance ID"));
            targetPanel.Children.Add(txtAttendanceId);
            targetPanel.Children.Add(CreateLabel("Student ID"));
            targetPanel.Children.Add(txtStudentId);
            targetPanel.Children.Add(CreateLabel("Date of Visit"));
            targetPanel.Children.Add(dpDate);
            targetPanel.Children.Add(CreateLabel("Time of Visit"));
            targetPanel.Children.Add(txtTime);

            var btnSubmit = new Button
            {
                Content = "Record Attendance",
                Margin = new Thickness(0, 10, 0, 0)
            };

            btnSubmit.Click += (s, e) =>
            {
                string attendanceId = txtAttendanceId.Text;
                string studentId = txtStudentId.Text;
                DateTime? visitDate = dpDate.SelectedDate;
                string timeInput = txtTime.Text;

                if (string.IsNullOrWhiteSpace(attendanceId) || string.IsNullOrWhiteSpace(studentId) || visitDate == null || string.IsNullOrWhiteSpace(timeInput))
                {
                    MessageBoxBuilder.ShowWarning("Please fill in all fields.");
                    return;
                }

                if (!TimeSpan.TryParse(timeInput, out TimeSpan visitTime))
                {
                    MessageBoxBuilder.ShowWarning("Invalid time format. Use HH:mm (e.g., 14:30)");
                    return;
                }

                try
                {
                    dbContext.usp_RecordVisit(attendanceId, studentId, visitDate.Value, visitTime);
                    MessageBoxBuilder.ShowSuccess("Attendance recorded.");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBoxBuilder.ShowError("An error occurred while recording attendance.\n\nDetails: " + ex.Message);
                }
            };

            targetPanel.Children.Add(btnSubmit);
        }


        public static void BuildAddUserForm(StackPanel targetPanel, NorthvilleLibraryDataContext db, Action onSuccess)
        {
            targetPanel.Children.Clear();

            var fields = new Dictionary<string, TextBox>();
            string[] textLabels = { "User ID", "Username", "Password", "Hash", "Role" };

            foreach (var label in textLabels)
            {
                string name = $"txt{label.Replace(" ", "")}";
                targetPanel.Children.Add(CreateLabel(label));
                var txt = CreateTextBox(name);
                fields[label] = txt;
                targetPanel.Children.Add(txt);
            }

            // IsActive and IsNew dropdowns
            var cbIsActive = CreateComboBox(new[] { "True", "False" });
            var cbIsNew = CreateComboBox(new[] { "True", "False" });

            targetPanel.Children.Add(CreateLabel("Is Active"));
            targetPanel.Children.Add(cbIsActive);
            targetPanel.Children.Add(CreateLabel("Is New"));
            targetPanel.Children.Add(cbIsNew);

            // Submit Button
            var btnSubmit = new Button
            {
                Content = "Add User",
                Margin = new Thickness(0, 10, 0, 0)
            };

            btnSubmit.Click += (s, e) =>
            {
                try
                {
                    db.usp_AddUser(
                        fields["User ID"].Text,
                        fields["Username"].Text,
                        fields["Password"].Text,
                        fields["Hash"].Text,
                        fields["Role"].Text,
                        bool.Parse(cbIsActive.SelectedItem.ToString()),
                        bool.Parse(cbIsNew.SelectedItem.ToString()),
                        DateTime.Now
                    );

                    MessageBoxBuilder.ShowSuccess("User successfully added.");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBoxBuilder.ShowError("An error occurred while adding the user.\n\nDetails: " + ex.Message);
                }
            };

            targetPanel.Children.Add(btnSubmit);
        }


        public static void BuildAddStudentForm(StackPanel targetPanel, NorthvilleLibraryDataContext db, Action onSuccess)
        {
            targetPanel.Children.Clear();

            // Student ID
            targetPanel.Children.Add(CreateLabel("Student ID"));
            var txtStudentId = CreateTextBox("txtStudentId");
            targetPanel.Children.Add(txtStudentId);

            // Student Name
            targetPanel.Children.Add(CreateLabel("Student Name"));
            var txtStudentName = CreateTextBox("txtStudentName");
            targetPanel.Children.Add(txtStudentName);

            // Course Name (ComboBox)
            targetPanel.Children.Add(CreateLabel("Course"));
            var cbCourse = CreateComboBox(db.courses.Select(c => c.course_name));
            targetPanel.Children.Add(cbCourse);

            // Contact Number
            targetPanel.Children.Add(CreateLabel("Contact Number"));
            var txtContactNum = CreateTextBox("txtContactNum");
            targetPanel.Children.Add(txtContactNum);

            // Submit Button
            var btnSubmit = new Button
            {
                Content = "Add Student",
                Margin = new Thickness(0, 10, 0, 0)
            };

            btnSubmit.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtStudentId.Text) ||
                    string.IsNullOrWhiteSpace(txtStudentName.Text) ||
                    string.IsNullOrWhiteSpace(txtContactNum.Text) ||
                    cbCourse.SelectedItem == null)
                {
                    MessageBoxBuilder.ShowWarning("Please fill in all fields.");
                    return;
                }

                try
                {
                    // Look up the course_id based on the selected course_name
                    var selectedCourseName = cbCourse.SelectedItem.ToString();
                    var course = db.courses.FirstOrDefault(c => c.course_name == selectedCourseName);

                    if (course == null)
                    {
                        MessageBoxBuilder.ShowError("Selected course not found.");
                        return;
                    }

                    var newStudent = new student
                    {
                        student_id = txtStudentId.Text.Trim(),
                        student_name = txtStudentName.Text.Trim(),
                        contact_number = txtContactNum.Text.Trim(),
                        course_id = course.course_id
                    };

                    db.students.InsertOnSubmit(newStudent);
                    db.SubmitChanges();

                    MessageBoxBuilder.ShowSuccess("Student successfully added.");
                    onSuccess?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBoxBuilder.ShowError("An error occurred while adding the student.\n\nDetails: " + ex.Message);
                }
            };

            targetPanel.Children.Add(btnSubmit);
        }


        public static void BuildEditBookCopyForm(StackPanel targetPanel, NorthvilleLibraryDataContext db, string copyId, Action onSaved)
        {
            targetPanel.Children.Clear();

            var copy = db.book_copies.FirstOrDefault(c => c.book_copy_id == copyId);
            if (copy == null)
            {
                MessageBoxBuilder.ShowError("Book copy not found.");
                return;
            }

            var edition = copy.book_edition;
            var book = edition.book;

            // --- BOOK FIELDS ---
            targetPanel.Children.Add(CreateLabel("Book Title"));
            var txtTitle = CreateTextBox("txtTitle", book.book_title);
            targetPanel.Children.Add(txtTitle);

            targetPanel.Children.Add(CreateLabel("Author"));
            var txtAuthor = CreateTextBox("txtAuthor", book.book_author);
            targetPanel.Children.Add(txtAuthor);

            // --- EDITION FIELDS ---
            targetPanel.Children.Add(CreateLabel("Edition Number"));
            var txtEdition = CreateTextBox("txtEdition", edition.edition_number.ToString());
            targetPanel.Children.Add(txtEdition);

            targetPanel.Children.Add(CreateLabel("Format"));
            var txtFormat = CreateTextBox("txtFormat", edition.book_format);
            targetPanel.Children.Add(txtFormat);

            // --- BOOK COPY FIELDS ---
            targetPanel.Children.Add(CreateLabel("Copy ID"));
            var txtCopyId = CreateTextBox("txtCopyId", copy.book_copy_id);
            txtCopyId.IsEnabled = false;
            targetPanel.Children.Add(txtCopyId);

            var statusList = db.book_status.Select(s => new { s.book_status_id, s.status_description }).ToList();
            var cbStatus = new ComboBox
            {
                ItemsSource = statusList,
                DisplayMemberPath = "status_description",
                SelectedValuePath = "book_status_id",
                SelectedValue = copy.book_status_id,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1d1f26")),
                Foreground = Brushes.White,
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 0, 15),
                BorderThickness = new Thickness(0)
            };
            targetPanel.Children.Add(CreateLabel("Book Status"));
            targetPanel.Children.Add(cbStatus);

            var cbLocation = CreateComboBox(
                db.book_locations.Select(l => l.location_id),
                copy.location_id
            );
            targetPanel.Children.Add(CreateLabel("Location"));
            targetPanel.Children.Add(cbLocation);

            var btnUpdate = new Button
            {
                Content = "Update Book Copy + Info",
                Margin = new Thickness(0, 10, 0, 0)
            };

            btnUpdate.Click += (s, e) =>
            {
                // Validate
                if (string.IsNullOrWhiteSpace(txtTitle.Text) ||
                    string.IsNullOrWhiteSpace(txtAuthor.Text) ||
                    string.IsNullOrWhiteSpace(txtEdition.Text) ||
                    string.IsNullOrWhiteSpace(txtFormat.Text) ||
                    cbStatus.SelectedValue == null ||
                    cbLocation.SelectedItem == null)
                {
                    MessageBoxBuilder.ShowWarning("Please complete all fields before saving.");
                    return;
                }

                if (!int.TryParse(txtEdition.Text, out int editionNum))
                {
                    MessageBoxBuilder.ShowWarning("Edition number must be a valid integer.");
                    return;
                }

                var confirm = MessageBoxBuilder.ShowConfirm("This update will apply to ALL book copies linked to this edition.\n\nAre you sure you want to proceed?", "System-Wide Book Update");
                if (confirm != MessageBoxResult.Yes)
                    return;

                try
                {
                    // Apply changes
                    book.book_title = txtTitle.Text.Trim();
                    book.book_author = txtAuthor.Text.Trim();
                    edition.edition_number = editionNum;
                    edition.book_format = txtFormat.Text.Trim();
                    copy.book_status_id = cbStatus.SelectedValue.ToString();
                    copy.location_id = cbLocation.SelectedItem.ToString();

                    db.SubmitChanges();

                    MessageBoxBuilder.ShowSuccess("Book, edition, and copy information updated successfully.");
                    onSaved?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBoxBuilder.ShowError("Update failed.\n\nDetails: " + ex.Message);
                }
            };

            targetPanel.Children.Add(btnUpdate);
        }



        #endregion

        #region Edit Form

        public static void BuildEditUserForm(StackPanel targetPanel, NorthvilleLibraryDataContext db, string userId, Action onSaved)
        {
            targetPanel.Children.Clear();

            var user = db.users.FirstOrDefault(u => u.user_id == userId);

            if (user == null)
            {
                MessageBoxBuilder.ShowError("User not found.");
                return;
            }

            // USER ID (readonly)
            targetPanel.Children.Add(CreateLabel("User ID"));
            var txtUserId = CreateTextBox("txtUserId", user.user_id);
            txtUserId.IsEnabled = false;
            targetPanel.Children.Add(txtUserId);

            // Username
            targetPanel.Children.Add(CreateLabel("Username"));
            var txtUsername = CreateTextBox("txtUsername", user.username);
            targetPanel.Children.Add(txtUsername);

            // Password
            targetPanel.Children.Add(CreateLabel("Password"));
            var txtPassword = CreateTextBox("txtPassword", user.user_password);
            targetPanel.Children.Add(txtPassword);

            // Role (dropdown)
            targetPanel.Children.Add(CreateLabel("Role"));
            var cbRole = CreateComboBox(new List<string> { "Librarian", "Clerical Assistant", "Student" }, user.user_role);
            targetPanel.Children.Add(cbRole);

            // Is Active (checkbox)
            var chkIsActive = new CheckBox { Content = "Is Active", IsChecked = user.is_active };
            targetPanel.Children.Add(chkIsActive);

            // Submit Button
            var btnUpdate = new Button
            {
                Content = "Update User",
                Margin = new Thickness(0, 10, 0, 0)
            };

            btnUpdate.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtUsername.Text) || string.IsNullOrWhiteSpace(txtPassword.Text) || cbRole.SelectedItem == null)
                {
                    MessageBoxBuilder.ShowWarning("Please complete all required fields.");
                    return;
                }

                user.username = txtUsername.Text;
                user.user_password = txtPassword.Text;
                user.user_role = cbRole.SelectedItem.ToString();
                user.is_active = chkIsActive.IsChecked ?? false;

                try
                {
                    CRUDHelper.SafeSubmit(db);
                    MessageBoxBuilder.ShowSuccess("User updated successfully.");
                    onSaved?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBoxBuilder.ShowError("Error updating user.\n\nDetails: " + ex.Message);
                }
            };

            targetPanel.Children.Add(btnUpdate);
        }


        public static void BuildEditStudentForm(StackPanel targetPanel, NorthvilleLibraryDataContext db, string studentId, Action onSaved)
        {
            targetPanel.Children.Clear();

            var student = db.students.FirstOrDefault(s => s.student_id == studentId);
            if (student == null)
            {
                MessageBoxBuilder.ShowError("Student not found.");
                return;
            }

            // Student ID (readonly)
            targetPanel.Children.Add(CreateLabel("Student ID"));
            var txtStudentId = CreateTextBox("txtStudentId", student.student_id);
            txtStudentId.IsEnabled = false;
            targetPanel.Children.Add(txtStudentId);

            // Student Name
            targetPanel.Children.Add(CreateLabel("Student Name"));
            var txtStudentName = CreateTextBox("txtStudentName", student.student_name);
            targetPanel.Children.Add(txtStudentName);

            // Course dropdown (bind course_id for update, show course_name)
            var courseList = db.courses.Select(c => new { c.course_id, c.course_name }).ToList();
            var cbCourse = new ComboBox
            {
                ItemsSource = courseList,
                DisplayMemberPath = "course_name",
                SelectedValuePath = "course_id",
                SelectedValue = student.course_id,
                Background = new SolidColorBrush((Color)ColorConverter.ConvertFromString("#1d1f26")),
                Foreground = Brushes.White,
                Padding = new Thickness(5),
                Margin = new Thickness(0, 0, 0, 15),
                BorderThickness = new Thickness(0)
            };
            targetPanel.Children.Add(CreateLabel("Course"));
            targetPanel.Children.Add(cbCourse);

            // Contact Number
            targetPanel.Children.Add(CreateLabel("Contact Number"));
            var txtContact = CreateTextBox("txtContact", student.contact_number);
            targetPanel.Children.Add(txtContact);

            // Save Button
            var btnSave = new Button
            {
                Content = "Update Student",
                Margin = new Thickness(0, 10, 0, 0)
            };

            btnSave.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtStudentName.Text) || cbCourse.SelectedValue == null)
                {
                    MessageBoxBuilder.ShowWarning("Please fill in all required fields.");
                    return;
                }

                try
                {
                    student.student_name = txtStudentName.Text;
                    student.contact_number = txtContact.Text;
                    student.course_id = cbCourse.SelectedValue.ToString();

                    db.SubmitChanges();
                    MessageBoxBuilder.ShowSuccess("Student successfully updated.");
                    onSaved?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBoxBuilder.ShowError("Update failed.\n\nDetails: " + ex.Message);
                }
            };

            targetPanel.Children.Add(btnSave);
        }


        public static void BuildEditCourseForm(StackPanel targetPanel, NorthvilleLibraryDataContext db, string courseId, Action onSaved)
        {
            targetPanel.Children.Clear();

            var course = db.courses.FirstOrDefault(c => c.course_id == courseId);
            if (course == null)
            {
                MessageBoxBuilder.ShowError("Course not found.");
                return;
            }

            // Course ID (readonly)
            targetPanel.Children.Add(CreateLabel("Course ID"));
            var txtCourseId = CreateTextBox("txtCourseId", course.course_id);
            txtCourseId.IsEnabled = false;
            targetPanel.Children.Add(txtCourseId);

            // Course Name
            targetPanel.Children.Add(CreateLabel("Course Name"));
            var txtCourseName = CreateTextBox("txtCourseName", course.course_name);
            targetPanel.Children.Add(txtCourseName);

            var btnUpdate = new Button
            {
                Content = "Update Course",
                Margin = new Thickness(0, 10, 0, 0)
            };

            btnUpdate.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtCourseName.Text))
                {
                    MessageBoxBuilder.ShowWarning("Course name cannot be empty.");
                    return;
                }

                try
                {
                    course.course_name = txtCourseName.Text;
                    db.SubmitChanges();
                    MessageBoxBuilder.ShowSuccess("Course updated successfully.");
                    onSaved?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBoxBuilder.ShowError("Update failed.\n\nDetails: " + ex.Message);
                }
            };

            targetPanel.Children.Add(btnUpdate);
        }

        public static void BuildEditBorrowForm(StackPanel targetPanel, NorthvilleLibraryDataContext db, string borrowId, Action onSaved)
        {
            targetPanel.Children.Clear();

            var borrow = db.borrow_transactions.FirstOrDefault(b => b.borrow_id == borrowId);
            if (borrow == null)
            {
                MessageBoxBuilder.ShowError("Borrow transaction not found.");
                return;
            }

            // Borrow ID (readonly)
            targetPanel.Children.Add(CreateLabel("Borrow ID"));
            var txtBorrowId = CreateTextBox("txtBorrowId", borrow.borrow_id);
            txtBorrowId.IsEnabled = false;
            targetPanel.Children.Add(txtBorrowId);

            // Student ID
            targetPanel.Children.Add(CreateLabel("Student ID"));
            var txtStudentId = CreateTextBox("txtStudentId", borrow.student_id);
            targetPanel.Children.Add(txtStudentId);

            // Book Copy ID
            targetPanel.Children.Add(CreateLabel("Book Copy ID"));
            var txtCopyId = CreateTextBox("txtCopyId", borrow.book_copy_id);
            targetPanel.Children.Add(txtCopyId);

            // Borrow Date
            targetPanel.Children.Add(CreateLabel("Borrow Date"));
            var dpBorrowDate = new DatePicker { SelectedDate = borrow.borrow_date };
            targetPanel.Children.Add(dpBorrowDate);

            // Return Date
            targetPanel.Children.Add(CreateLabel("Return Date"));
            var dpReturnDate = new DatePicker { SelectedDate = borrow.return_date };
            targetPanel.Children.Add(dpReturnDate);

            var btnUpdate = new Button
            {
                Content = "Update Borrow Record",
                Margin = new Thickness(0, 10, 0, 0)
            };

            btnUpdate.Click += (s, e) =>
            {
                if (string.IsNullOrWhiteSpace(txtStudentId.Text) || string.IsNullOrWhiteSpace(txtCopyId.Text))
                {
                    MessageBoxBuilder.ShowWarning("All fields must be filled.");
                    return;
                }

                try
                {
                    borrow.student_id = txtStudentId.Text;
                    borrow.book_copy_id = txtCopyId.Text;
                    borrow.borrow_date = dpBorrowDate.SelectedDate ?? DateTime.Today;
                    borrow.return_date = dpReturnDate.SelectedDate;

                    db.SubmitChanges();
                    MessageBoxBuilder.ShowSuccess("Borrow record updated.");
                    onSaved?.Invoke();
                }
                catch (Exception ex)
                {
                    MessageBoxBuilder.ShowError("Update failed.\n\nDetails: " + ex.Message);
                }
            };

            targetPanel.Children.Add(btnUpdate);
        }


        #endregion
    }
}
