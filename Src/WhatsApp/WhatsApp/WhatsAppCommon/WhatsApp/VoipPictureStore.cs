// Decompiled with JetBrains decompiler
// Type: WhatsApp.VoipPictureStore
// Assembly: WhatsAppCommon, Version=2.18.370.0, Culture=neutral, PublicKeyToken=null
// MVID: 1D438F5E-0D32-4352-9FB4-5035480A3050
// Assembly location: C:\Users\Admin\Desktop\RE\WABeta\WhatsAppCommon.dll

using System;
using System.IO;
using System.IO.IsolatedStorage;
using System.Threading;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;
using Windows.ApplicationModel;
using Windows.Storage;


namespace WhatsApp
{
  public class VoipPictureStore
  {
    private const string DirectoryPathWP81 = "voipPictures";
    private const string DirectoryPathWP10 = "voipPicturesSq";
    private static string DirectoryPath = AppState.IsWP10OrLater ? "voipPicturesSq" : "voipPictures";
    private static string DirectoryPathOtherOS = AppState.IsWP10OrLater ? "voipPictures" : "voipPicturesSq";
    private const int ImageWidth = 480;
    private static int ImageHeight = AppState.IsWP10OrLater ? 480 : 550;
    private static string LogHdr = "VoipPS";
    private static string defaultContactIconRelativePath = ImageStore.IsDarkTheme() ? "/Images/assets/dark/default-contact-icon-480x480.jpg" : "/Images/assets/light/default-contact-icon-480x480.jpg";
    public static string DefaultContactIcon = Package.Current.InstalledLocation.Path + VoipPictureStore.defaultContactIconRelativePath;
    private static string defaultContactIcon240x480RelativePath = ImageStore.IsDarkTheme() ? "/Images/assets/dark/default-contact-icon-240x480.jpg" : "/Images/assets/light/default-contact-icon-240x480.jpg";
    private static string defaultContactIcon240x240RelativePath = ImageStore.IsDarkTheme() ? "/Images/assets/dark/default-contact-icon-240x240.jpg" : "/Images/assets/light/default-contact-icon-240x240.jpg";
    private const int imageGapSize = 2;

    public static void EnsureVoipContactPhoto(string jid)
    {
      UserStatus userStatus = UserCache.Get(jid, false);
      if (userStatus == null)
        return;
      string contactPhotoFilepath = VoipPictureStore.GenerateVoipContactPhotoFilepath(jid);
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        if (storeForApplication.FileExists(contactPhotoFilepath))
          return;
        string photoFilepathOtherOs = VoipPictureStore.GenerateVoipContactPhotoFilepathOtherOS(jid);
        if (storeForApplication.FileExists(photoFilepathOtherOs))
          storeForApplication.DeleteFile(photoFilepathOtherOs);
      }
      bool toRequestLargePic = false;
      string profilePhotoPath = ChatPictureStore.GetPicturePath(jid, out toRequestLargePic) ?? userStatus.PhotoPath;
      VoipPictureStore.CreateVoipContactPhoto(contactPhotoFilepath, profilePhotoPath);
      if (!toRequestLargePic)
        return;
      AppState.SchedulePersistentAction(PersistentAction.SendGetImage(userStatus.Jid));
    }

    public static string GetVoipContactPhotoPath(string jid)
    {
      string contactPhotoPath = (string) null;
      string contactPhotoFilepath = VoipPictureStore.GenerateVoipContactPhotoFilepath(jid);
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        if (storeForApplication.FileExists(contactPhotoFilepath))
          contactPhotoPath = ApplicationData.Current.LocalFolder.Path + "/" + contactPhotoFilepath;
      }
      if (contactPhotoPath == null)
        contactPhotoPath = VoipPictureStore.DefaultContactIcon;
      return contactPhotoPath;
    }

    private static string GenerateVoipContactPhotoFilepath(string id)
    {
      return string.Format("{0}/{1}.jpg", (object) VoipPictureStore.DirectoryPath, (object) id);
    }

    private static string GenerateVoipContactPhotoFilepathOtherOS(string id)
    {
      return string.Format("{0}/{1}.jpg", (object) VoipPictureStore.DirectoryPathOtherOS, (object) id);
    }

    public static void DeleteVoipContactPhoto(string jid)
    {
      using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
      {
        string contactPhotoFilepath = VoipPictureStore.GenerateVoipContactPhotoFilepath(jid);
        if (storeForApplication.FileExists(contactPhotoFilepath))
          storeForApplication.DeleteFile(contactPhotoFilepath);
        string photoFilepathOtherOs = VoipPictureStore.GenerateVoipContactPhotoFilepathOtherOS(jid);
        if (!storeForApplication.FileExists(photoFilepathOtherOs))
          return;
        storeForApplication.DeleteFile(photoFilepathOtherOs);
      }
    }

    private static void CreateVoipContactPhoto(string outputFilePath, string profilePhotoPath)
    {
      WriteableBitmap writeableBitmap = BitmapUtils.LoadFromFile(profilePhotoPath, 480, 480);
      if (writeableBitmap == null)
        return;
      Image image1 = new Image();
      image1.Source = (ImageSource) writeableBitmap;
      image1.Stretch = Stretch.UniformToFill;
      image1.Width = 480.0;
      image1.Height = (double) VoipPictureStore.ImageHeight;
      Image image2 = image1;
      Rectangle rectangle1 = new Rectangle();
      rectangle1.Width = 480.0;
      rectangle1.Height = (double) VoipPictureStore.ImageHeight;
      LinearGradientBrush linearGradientBrush = new LinearGradientBrush();
      GradientStopCollection gradientStopCollection = new GradientStopCollection();
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Color.FromArgb((byte) 128, (byte) 0, (byte) 0, (byte) 0),
        Offset = 0.0
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Color.FromArgb((byte) 64, (byte) 0, (byte) 0, (byte) 0),
        Offset = 0.3
      });
      gradientStopCollection.Add(new GradientStop()
      {
        Color = Colors.Transparent,
        Offset = 0.45
      });
      linearGradientBrush.GradientStops = gradientStopCollection;
      linearGradientBrush.StartPoint = new System.Windows.Point(0.0, 0.0);
      linearGradientBrush.EndPoint = new System.Windows.Point(0.0, 1.0);
      rectangle1.Fill = (Brush) linearGradientBrush;
      Rectangle rectangle2 = rectangle1;
      Canvas canvas = new Canvas();
      canvas.Width = 480.0;
      canvas.Height = (double) VoipPictureStore.ImageHeight;
      Canvas element = canvas;
      element.Children.Add((UIElement) image2);
      element.Children.Add((UIElement) rectangle2);
      WriteableBitmap bitmap = new WriteableBitmap(480, VoipPictureStore.ImageHeight);
      bitmap.Render((UIElement) element, (Transform) null);
      bitmap.Invalidate();
      image2.Source = (ImageSource) null;
      MemoryStream jpegStream = bitmap.ToJpegStream(-1, new int?(80));
      if (jpegStream == null)
        return;
      using (jpegStream)
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          if (storeForApplication.FileExists(outputFilePath))
            storeForApplication.DeleteFile(outputFilePath);
          if (!storeForApplication.DirectoryExists(VoipPictureStore.DirectoryPath))
            storeForApplication.CreateDirectory(VoipPictureStore.DirectoryPath);
          using (IsolatedStorageFileStream destination = storeForApplication.OpenFile(outputFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
            jpegStream.CopyTo((Stream) destination);
          if (!storeForApplication.DirectoryExists(VoipPictureStore.DirectoryPathOtherOS))
            return;
          try
          {
            string[] strArray = storeForApplication.GetFileNames(VoipPictureStore.DirectoryPathOtherOS + "/*") ?? new string[0];
            Log.l(VoipPictureStore.LogHdr, "Found old format directory with {0} files", (object) strArray.Length);
            foreach (string str in strArray)
              storeForApplication.DeleteFile(string.Format("{0}/{1}", (object) VoipPictureStore.DirectoryPathOtherOS, (object) str));
            storeForApplication.DeleteDirectory(VoipPictureStore.DirectoryPathOtherOS);
          }
          catch (Exception ex)
          {
            Log.l(VoipPictureStore.LogHdr, "Exception {0}", (object) ex.GetFriendlyMessage());
          }
        }
      }
    }

    public static string GetVoipContactsPhotoPath(string[] jids)
    {
      if (jids == null || jids.Length < 1)
      {
        Log.l(VoipPictureStore.LogHdr, "GetVoipContactsPhotoPath unexpectedly called with no jids: {0}", jids == null ? (object) "null" : (object) "0");
        return VoipPictureStore.DefaultContactIcon;
      }
      if (jids.Length == 1)
      {
        Log.l(VoipPictureStore.LogHdr, "GetVoipContactsPhotoPath unexpectedly called with one jid");
        return VoipPictureStore.GetVoipContactPhotoPath(jids[0]);
      }
      long ticks = DateTime.Now.Ticks;
      int initialCount = Math.Min(jids.Length, 4);
      CountdownEvent cde = new CountdownEvent(initialCount);
      string[] photoPaths = new string[initialCount];
      string contactsPhotoPath = VoipPictureStore.DefaultContactIcon;
      try
      {
        for (int index = 0; index < initialCount; ++index)
        {
          int localI = index;
          WAThreadPool.QueueUserWorkItem((Action) (() =>
          {
            try
            {
              photoPaths[localI] = VoipPictureStore.GetVoipContactPhotoPath(jids[localI]);
            }
            catch (Exception ex)
            {
              Log.l(VoipPictureStore.LogHdr, "Exception creating profile pic for jid {0}", (object) ex.GetFriendlyMessage());
              photoPaths[localI] = VoipPictureStore.DefaultContactIcon;
            }
            finally
            {
              cde?.Signal();
            }
          }));
        }
        cde.Wait(500);
        if (cde.CurrentCount >= 2)
        {
          contactsPhotoPath = VoipPictureStore.DefaultContactIcon;
        }
        else
        {
          Log.d(VoipPictureStore.LogHdr, "Created paths for {0} of {1}", (object) (initialCount - cde.CurrentCount), (object) initialCount);
          string contactPhotoFilepath = VoipPictureStore.GenerateVoipContactPhotoFilepath("groupcall");
          VoipPictureStore.CreateVoipContactsPhoto(contactPhotoFilepath, photoPaths);
          contactsPhotoPath = ApplicationData.Current.LocalFolder.Path + "/" + contactPhotoFilepath;
        }
      }
      catch (Exception ex)
      {
        Log.SendCrashLog(ex, "exception creating group call image", logOnlyForRelease: true);
        contactsPhotoPath = VoipPictureStore.DefaultContactIcon;
      }
      finally
      {
        cde.SafeDispose();
        cde = (CountdownEvent) null;
      }
      Log.d(VoipPictureStore.LogHdr, "Took {0}ms to create {1}", (object) ((DateTime.Now.Ticks - ticks) / 10000L), (object) contactsPhotoPath);
      return contactsPhotoPath;
    }

    private static void CreateVoipContactsPhoto(string outputFilePath, string[] voipPhotoPaths)
    {
      int imageCount = Math.Min(voipPhotoPaths.Length, 4);
      CountdownEvent cde = new CountdownEvent(imageCount);
      WriteableBitmap[] images = new WriteableBitmap[voipPhotoPaths.Length];
      WriteableBitmap wb = (WriteableBitmap) null;
      try
      {
        Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
        {
          wb = new WriteableBitmap(480, VoipPictureStore.ImageHeight);
          for (int index = 0; index < imageCount; ++index)
          {
            if (cde == null)
              break;
            try
            {
              if (voipPhotoPaths[index] != null)
              {
                if (voipPhotoPaths[index] != VoipPictureStore.DefaultContactIcon)
                {
                  try
                  {
                    images[index] = BitmapUtils.LoadFromFile(voipPhotoPaths[index], 480, 480);
                  }
                  catch (Exception ex)
                  {
                    Log.l(VoipPictureStore.LogHdr, "Exception retrieving profile pic for jid {0}", (object) ex.GetFriendlyMessage());
                  }
                }
              }
              if (images[index] == null)
              {
                string uriString = VoipPictureStore.defaultContactIcon240x240RelativePath;
                if (imageCount == 2 || imageCount == 3 && index == 0)
                  uriString = VoipPictureStore.defaultContactIcon240x480RelativePath;
                BitmapImage source = new BitmapImage(new Uri(uriString, UriKind.Relative))
                {
                  CreateOptions = BitmapCreateOptions.None
                };
                images[index] = new WriteableBitmap((BitmapSource) source);
              }
            }
            catch (Exception ex)
            {
              Log.l(VoipPictureStore.LogHdr, "Exception retrieving profile pic: {0}", (object) ex.GetFriendlyMessage());
              images[index] = (WriteableBitmap) null;
            }
            finally
            {
              cde?.Signal();
            }
          }
        }));
        cde.Wait(1000);
        Log.d(VoipPictureStore.LogHdr, "looking for {0} images, left {1}", (object) imageCount, (object) cde.CurrentCount);
        int[] numArray = wb != null ? wb.Pixels : throw new TimeoutException("Could not get bitmap created in time");
        int num1 = ImageStore.IsDarkTheme() ? 2039583 : 14540253;
        for (int index = 0; index < 480; ++index)
          numArray[index] = num1;
        for (int index = 1; index < VoipPictureStore.ImageHeight; index += index)
        {
          int num2 = index * 480;
          Array.Copy((Array) numArray, 0, (Array) numArray, num2, Math.Min(num2, numArray.Length - num2));
        }
        if (imageCount == 2 || imageCount == 3)
          VoipPictureStore.AddOneImage(wb, images[0], true);
        else
          VoipPictureStore.AddTwoImages(wb, images[0], images[1], true);
        if (imageCount == 2)
          VoipPictureStore.AddOneImage(wb, images[1], false);
        else if (imageCount == 3)
          VoipPictureStore.AddTwoImages(wb, images[1], images[2], false);
        else
          VoipPictureStore.AddTwoImages(wb, images[2], images[3], false);
        Log.d(VoipPictureStore.LogHdr, "created composite image");
        for (int index = 0; index < images.Length; ++index)
          images[index] = (WriteableBitmap) null;
      }
      finally
      {
        CountdownEvent d = cde;
        cde = (CountdownEvent) null;
        d.SafeDispose();
      }
      WriteableBitmap bitmap = wb;
      MemoryStream jpegStream = bitmap != null ? bitmap.ToJpegStream(-1, new int?(80)) : (MemoryStream) null;
      if (jpegStream == null)
        return;
      Log.d(VoipPictureStore.LogHdr, "Saving composite to {0}", (object) outputFilePath);
      using (jpegStream)
      {
        using (IsolatedStorageFile storeForApplication = IsolatedStorageFile.GetUserStoreForApplication())
        {
          if (storeForApplication.FileExists(outputFilePath))
            storeForApplication.DeleteFile(outputFilePath);
          if (!storeForApplication.DirectoryExists(VoipPictureStore.DirectoryPath))
            storeForApplication.CreateDirectory(VoipPictureStore.DirectoryPath);
          using (IsolatedStorageFileStream destination = storeForApplication.OpenFile(outputFilePath, FileMode.Create, FileAccess.ReadWrite, FileShare.ReadWrite | FileShare.Delete))
            jpegStream.CopyTo((Stream) destination);
          if (!storeForApplication.DirectoryExists(VoipPictureStore.DirectoryPathOtherOS))
            return;
          try
          {
            string[] strArray = storeForApplication.GetFileNames(VoipPictureStore.DirectoryPathOtherOS + "/*") ?? new string[0];
            Log.l(VoipPictureStore.LogHdr, "Found old format directory with {0} files", (object) strArray.Length);
            foreach (string str in strArray)
              storeForApplication.DeleteFile(string.Format("{0}/{1}", (object) VoipPictureStore.DirectoryPathOtherOS, (object) str));
            storeForApplication.DeleteDirectory(VoipPictureStore.DirectoryPathOtherOS);
          }
          catch (Exception ex)
          {
            Log.l(VoipPictureStore.LogHdr, "Exception {0}", (object) ex.GetFriendlyMessage());
          }
        }
      }
    }

    private static bool AddOneImage(
      WriteableBitmap output,
      WriteableBitmap input,
      bool leftHandSide)
    {
      if (output == null || input == null)
      {
        Log.l(VoipPictureStore.LogHdr, "Output and input images must exist {0}, {1}", (object) (output == null), (object) (input == null));
        return false;
      }
      int val1 = Math.Min(output.PixelHeight, output.PixelWidth);
      try
      {
        if (output.PixelWidth != input.PixelWidth || output.PixelHeight != input.PixelHeight || output.PixelWidth != val1 || output.PixelHeight != val1)
        {
          int[] pixels1 = input.Pixels;
          int[] pixels2 = output.Pixels;
          int num1 = Math.Min(val1, input.PixelHeight);
          int num2 = Math.Min(val1 / 2, input.PixelWidth);
          int num3 = val1;
          int pixelWidth = input.PixelWidth;
          int num4 = (output.PixelHeight - num1) / 2;
          int num5 = (input.PixelHeight - num1) / 2;
          int num6 = leftHandSide ? output.PixelWidth / 2 - num2 : output.PixelWidth / 2 + 2;
          int num7 = (input.PixelWidth - num2) / 2;
          for (int index1 = 0; index1 < num1; ++index1)
          {
            int num8 = (index1 + num4) * num3 + num6;
            int num9 = (index1 - num5) * pixelWidth + num7;
            for (int index2 = 0; index2 < num2 - 2; ++index2)
              pixels2[index2 + num8] = pixels1[index2 + num9];
          }
        }
        else
        {
          int num10 = val1;
          int num11 = val1;
          int num12 = num11 / 2;
          int[] pixels3 = input.Pixels;
          int[] pixels4 = output.Pixels;
          int num13 = leftHandSide ? 0 : num12 + 2;
          int num14 = num11 / 4;
          for (int index3 = 0; index3 < num10; ++index3)
          {
            int num15 = index3 * num11;
            for (int index4 = 0; index4 < num12 - 2; ++index4)
              pixels4[index4 + num15 + num13] = pixels3[index4 + num15 + num14];
          }
        }
      }
      catch (Exception ex)
      {
        Log.l(VoipPictureStore.LogHdr, "Details: output {0}x{1} input {2}x{3}, edge {4}", (object) output.PixelWidth, (object) output.PixelHeight, (object) input.PixelWidth, (object) input.PixelHeight, (object) val1);
        Log.SendCrashLog(ex, "Exception creating group call image", logOnlyForRelease: true);
      }
      return true;
    }

    private static bool AddTwoImages(
      WriteableBitmap output,
      WriteableBitmap input1,
      WriteableBitmap input2,
      bool leftHandSide)
    {
      if (output == null)
      {
        Log.l(VoipPictureStore.LogHdr, "Output must exist");
        return false;
      }
      bool flag = true;
      int num1 = Math.Min(output.PixelHeight, output.PixelWidth);
      if (input1 != null && input1.PixelHeight > 0)
      {
        if (input1.PixelWidth > 0)
        {
          try
          {
            if (output.PixelWidth == input1.PixelWidth && output.PixelHeight == input1.PixelHeight && output.PixelWidth == num1 && output.PixelHeight == num1)
            {
              int pixelHeight = output.PixelHeight;
              int pixelWidth = output.PixelWidth;
              int num2 = pixelWidth / 2;
              int num3 = pixelHeight / 2;
              int[] pixels1 = input1.Pixels;
              int[] pixels2 = output.Pixels;
              int num4 = leftHandSide ? 0 : num2 + 2;
              int num5 = pixelWidth / 4;
              for (int index1 = 0; index1 < num3 - 2; ++index1)
              {
                int num6 = pixelWidth * (pixelHeight / 4 + index1) + num5;
                int num7 = index1 * pixelWidth + num4;
                for (int index2 = 0; index2 < num2 - 2; ++index2)
                  pixels2[index2 + num7] = pixels1[index2 + num6];
              }
              goto label_19;
            }
            else
            {
              int num8 = Math.Min(num1 / 2, input1.PixelHeight);
              int num9 = Math.Min(num1 / 2, input1.PixelWidth);
              int[] pixels3 = input1.Pixels;
              int[] pixels4 = output.Pixels;
              int pixelWidth1 = output.PixelWidth;
              int pixelWidth2 = input1.PixelWidth;
              int num10 = output.PixelHeight / 2 - num8;
              int num11 = (input1.PixelHeight - num8) / 2;
              int num12 = leftHandSide ? output.PixelWidth / 2 - num9 : output.PixelWidth / 2 + 2;
              int num13 = (input1.PixelWidth - num9) / 2;
              for (int index3 = 0; index3 < num8 - 2; ++index3)
              {
                int num14 = (index3 + num10) * pixelWidth1 + num12;
                int num15 = (index3 + num11) * pixelWidth2 + num13;
                for (int index4 = 0; index4 < num9 - 2; ++index4)
                  pixels4[index4 + num14] = pixels3[index4 + num15];
              }
              goto label_19;
            }
          }
          catch (Exception ex)
          {
            Log.l(VoipPictureStore.LogHdr, "Details: Output {0}x{1} Input1 {2}x{3}, edge {4}", (object) output.PixelWidth, (object) output.PixelHeight, (object) input1.PixelWidth, (object) input1.PixelHeight, (object) num1);
            Log.SendCrashLog(ex, "Exception creating group call image", logOnlyForRelease: true);
            goto label_19;
          }
        }
      }
      Log.l(VoipPictureStore.LogHdr, "Input1 should exist or have pixels {0}", (object) (input1 != null));
      flag = false;
label_19:
      if (input2 != null && input2.PixelHeight > 0)
      {
        if (input2.PixelWidth > 0)
        {
          try
          {
            if (output.PixelWidth == input2.PixelWidth && output.PixelHeight == input2.PixelHeight && output.PixelWidth == num1 && output.PixelHeight == num1)
            {
              int pixelHeight = output.PixelHeight;
              int pixelWidth = output.PixelWidth;
              int num16 = pixelWidth / 2;
              int num17 = pixelHeight / 2;
              int[] pixels5 = input2.Pixels;
              int[] pixels6 = output.Pixels;
              int num18 = leftHandSide ? 0 : num16 + 2;
              int num19 = pixelWidth / 4;
              for (int index5 = 2; index5 < num17; ++index5)
              {
                int num20 = pixelWidth * (pixelHeight / 4 + index5) + num19;
                int num21 = (index5 + num17) * pixelWidth + num18;
                for (int index6 = 0; index6 < num16 - 2; ++index6)
                  pixels6[index6 + num21] = pixels5[index6 + num20];
              }
              goto label_36;
            }
            else
            {
              int num22 = Math.Min(num1 / 2, input2.PixelHeight);
              int num23 = Math.Min(num1 / 2, input2.PixelWidth);
              int[] pixels7 = input2.Pixels;
              int[] pixels8 = output.Pixels;
              int pixelWidth3 = output.PixelWidth;
              int pixelWidth4 = input2.PixelWidth;
              int num24 = output.PixelHeight / 2;
              int num25 = (input2.PixelHeight - num22) / 2;
              int num26 = leftHandSide ? output.PixelWidth / 2 - num23 : output.PixelWidth / 2 + 2;
              int num27 = (input2.PixelWidth - num23) / 2;
              for (int index7 = 2; index7 < num22; ++index7)
              {
                int num28 = (index7 + num24) * pixelWidth3 + num26;
                int num29 = (index7 + num25) * pixelWidth4 + num27;
                for (int index8 = 0; index8 < num23 - 2; ++index8)
                  pixels8[index8 + num28] = pixels7[index8 + num29];
              }
              goto label_36;
            }
          }
          catch (Exception ex)
          {
            Log.l(VoipPictureStore.LogHdr, "Details: Output {0}x{1} Input2 {2}x{3}, edge {4}", (object) output.PixelWidth, (object) output.PixelHeight, (object) input2.PixelWidth, (object) input2.PixelHeight, (object) num1);
            Log.SendCrashLog(ex, "Exception creating group call image", logOnlyForRelease: true);
            goto label_36;
          }
        }
      }
      Log.l(VoipPictureStore.LogHdr, "Input2 should exist or have pixels {0}", (object) (input2 != null));
      flag = false;
label_36:
      return flag;
    }

    public static void EnsureVoipContactsPhoto(string[] jids)
    {
      if (jids == null || jids.Length <= 1)
        return;
      WAThreadPool.QueueUserWorkItem((Action) (() => Deployment.Current.Dispatcher.BeginInvokeIfNeeded((Action) (() =>
      {
        for (int index = 0; index < jids.Length; ++index)
          VoipPictureStore.EnsureVoipContactPhoto(jids[index]);
      }))));
    }
  }
}
