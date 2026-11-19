using System;
using System.IO;
using Avalonia.Controls;
using Avalonia.Interactivity;
using Avalonia.Media;
using Avalonia.Media.Imaging;
using Avalonia.Platform.Storage;
using InformationSystemForTrackingExpenses.Repositories;
using InformationSystemForTrackingExpenses.Tables;

namespace InformationSystemForTrackingExpenses.Views
{
    public partial class ProfileWindow : Window
    {
        private readonly UsersRep _usersRep = new();
        private readonly UsersTable _user;
        private string? _pendingAvatarPath;

        public UsersTable? UpdatedUser { get; private set; }

        public ProfileWindow(UsersTable user)
        {
            InitializeComponent();
            _user = user;
            _pendingAvatarPath = user.avatar_path;

            RoleSubtitleText.Text = $"Role: {user.role}";
            NameBox.Text = user.name;
            EmailBox.Text = user.email;
            LoadBitmapToImage(AvatarPreviewImage, _pendingAvatarPath);
        }

        private static void LoadBitmapToImage(Image target, string? path)
        {
            try
            {
                if (!string.IsNullOrWhiteSpace(path) && File.Exists(path))
                {
                    using var stream = File.OpenRead(path);
                    target.Source = new Bitmap(stream);
                    return;
                }
            }
            catch
            {
                // Очищаем при ошибке чтения файла
            }
            target.Source = null;
        }

        private async void OnSelectAvatarClick(object? sender, RoutedEventArgs e)
        {
            var files = await StorageProvider.OpenFilePickerAsync(new FilePickerOpenOptions
            {
                Title = "Select Profile Photo",
                AllowMultiple = false,
                FileTypeFilter = new[]
                {
                    new FilePickerFileType("Images")
                    {
                        Patterns = new[] { "*.png", "*.jpg", "*.jpeg", "*.webp", "*.bmp" }
                    }
                }
            });

            if (files.Count == 0) return;

            string? sourcePath = files[0].TryGetLocalPath();
            if (string.IsNullOrWhiteSpace(sourcePath) || !File.Exists(sourcePath)) return;

            string avatarsDir = Path.Combine(AppContext.BaseDirectory, "Avatars");
            Directory.CreateDirectory(avatarsDir);

            string ext = Path.GetExtension(sourcePath);
            string destFileName = $"user_{_user.id}_{DateTime.Now:yyyyMMddHHmmss}{ext}";
            string destPath = Path.Combine(avatarsDir, destFileName);

            File.Copy(sourcePath, destPath, overwrite: true);
            _pendingAvatarPath = destPath;
            LoadBitmapToImage(AvatarPreviewImage, _pendingAvatarPath);
            StatusText.IsVisible = false;
        }

        private void OnRemoveAvatarClick(object? sender, RoutedEventArgs e)
        {
            _pendingAvatarPath = null;
            AvatarPreviewImage.Source = null;
            StatusText.IsVisible = false;
        }

        private void OnSaveClick(object? sender, RoutedEventArgs e)
        {
            string name = NameBox.Text?.Trim() ?? "";
            string email = EmailBox.Text?.Trim() ?? "";
            string currentPass = CurrentPasswordBox.Text ?? "";
            string newPass = NewPasswordBox.Text ?? "";

            if (string.IsNullOrWhiteSpace(name) || string.IsNullOrWhiteSpace(email))
            {
                ShowStatus("Name and Email cannot be empty.", isError: true);
                return;
            }

            var existingWithEmail = _usersRep.GetByEmail(email);
            if (existingWithEmail != null && existingWithEmail.id != _user.id)
            {
                ShowStatus("This email is already taken by another user.", isError: true);
                return;
            }

            bool changePassword = !string.IsNullOrEmpty(newPass);
            if (changePassword)
            {
                string currentHash = UsersRep.HashPassword(currentPass);
                if (!string.Equals(currentHash, _user.password_hash, StringComparison.OrdinalIgnoreCase))
                {
                    ShowStatus("Current password is incorrect.", isError: true);
                    return;
                }
            }

            var updated = new UsersTable(
                _user.id,
                name,
                email,
                changePassword ? newPass : _user.password_hash,
                _user.role,
                _user.created_at,
                _pendingAvatarPath
            );

            if (!_usersRep.Update(updated, hashNewPassword: changePassword))
            {
                ShowStatus("Failed to save profile changes.", isError: true);
                return;
            }

            UpdatedUser = _usersRep.GetbyID(_user.id) ?? updated;
            Close();
        }

        private void OnCancelClick(object? sender, RoutedEventArgs e)
        {
            UpdatedUser = null;
            Close();
        }

        private void ShowStatus(string message, bool isError)
        {
            StatusText.Text = message;
            StatusText.Foreground = isError
                ? Brush.Parse("#FF8A8A")
                : Brush.Parse("#7ED987");
            StatusText.IsVisible = true;
        }
    }
}