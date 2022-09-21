using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Security;
using System.Text;
using System.Threading.Tasks;
using Microsoft.VisualBasic;
using System.Net;
using System.Data.SqlClient;

public class Frm_Login
{
    Frm_Login()
    {
        AppUpdate = new WebClient();
    }

    private string dircpath = My.Application.Info.DirectoryPath;
    private string ToolUpdate = dircpath + @"\Update.zip";
    private WebClient _AppUpdate;

    private WebClient AppUpdate
    {
        [MethodImpl(MethodImplOptions.Synchronized)]
        get
        {
            return _AppUpdate;
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        set
        {
            if (_AppUpdate != null)
            {
                _AppUpdate.DownloadProgressChanged -= downloader_DownloadProgressChanged;
                _AppUpdate.DownloadFileCompleted -= DownloadComplete;
            }

            _AppUpdate = value;
            if (_AppUpdate != null)
            {
                _AppUpdate.DownloadProgressChanged += downloader_DownloadProgressChanged;
                _AppUpdate.DownloadFileCompleted += DownloadComplete;
            }
        }
    }

    private SqlConnection conexion = new SqlConnection("Data Source=DESKTOP-VR73LN6;Persist Security Info=True;Initial Catalog=sishallo;Integrated Security=False;User ID=sa;password=02129600;");
    // Dim conexion As New SqlConnection(Cb_Conexion.Text.ToString)
    private SqlCommand cmd = new SqlCommand();
    private SqlDataAdapter da = new SqlDataAdapter();
    private SqlDataReader reader;
    private DataSet ds = new DataSet(), ds1 = new DataSet();

    private void downloader_DownloadProgressChanged(object sender, DownloadProgressChangedEventArgs e)
    {
        UpdateStatus.Value = e.ProgressPercentage;
        LbUpdate.Text = e.ProgressPercentage.ToString() + "%";
        LbUpdateSize.Text = string.Format("{0} Mbs / {1} Mbs", (e.BytesReceived / (double)1024M / (double)1024M).ToString("0.00"), (e.TotalBytesToReceive / (double)1024M / (double)1024M).ToString("0.00"));
    }
    private void DownloadComplete(object sender, System.ComponentModel.AsyncCompletedEventArgs e)
    {
        Interaction.MsgBox("Actualizacion Terminada.", MsgBoxStyle.Information, "Update");
        Application.Exit();
    }
    private void CheckForUpdates()
    {
        ServicePointManager.Expect100Continue = true;
        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
        System.Net.HttpWebRequest request = System.Net.HttpWebRequest.Create("");
        System.Net.HttpWebResponse response = request.GetResponse();
        System.IO.StreamReader sr = new System.IO.StreamReader(response.GetResponseStream());
        string newestversion = sr.ReadToEnd();
        string currentversion = Application.ProductVersion;


        if (newestversion.Contains(currentversion))
            Interaction.MsgBox("Su programa se encuentra en la ultima version disponible!", MsgBoxStyle.Information, "Programa en ultima version!");
        else
        {
            MsgBoxResult ask = MsgBox("Hay nueva actualizacion, quieres descargarla?", MessageBoxButtons.YesNo, "Nueva actualizacion!");
            if (ask == MsgBoxResult.Yes)
            {
                FrmUpdate();
                AppUpdate.DownloadFileAsync(new Uri(""), ToolUpdate);
            }
            else
                try
                {
                    // En el caso que no, solo replanzara el antiguo changelog por el nuevo
                    Interaction.MsgBox("Revise siempre los cambios del nuevo parche antes de actualizar!", MsgBoxStyle.Information, "Programa en ultima version!");
                    this.Show();
                }
                catch (Exception ex)
                {
                    Interaction.MsgBox("Changelog descargado!", MsgBoxStyle.Information, "Aviso");
                }
        }
    }

    private void FrmUpdate()
    {
        TabControl1.TabPages(0).Enabled = false;
        TabControl1.TabPages(1).Enabled = false;
    }
    private void Button3_Click(object sender, EventArgs e)
    {
        Interaction.MsgBox("Todas las acciones realizadas en este sistema son auditadas por el departamento de cómputos de esta empresa, cualquier acción de tipo maliciosa o fraudulenta será detectada al instante y se tomarán las acciones correctivas de lugar." + Constants.vbNewLine + Constants.vbNewLine + "Este programa está debidamente registrado, su uso o distribución ilegal conlleva a sanciones drásticas de la Ley.", MsgBoxStyle.Information, "Aviso");
    }

    private void Frm_Login_Load(object sender, EventArgs e)
    {
        conexion.ConnectionString = Cb_Conexion.Text.ToString;
        // CheckConnection()
        // FrmNormal()
        // CheckKey()
        // Decrypt()
        this.Text = "Sistema de administracion y gestion de inventario" + " " + Application.ProductVersion + " " + "Test Edition";
    }

    private void btncheck_Click(object sender, EventArgs e)
    {
    }

    private void btnCheckConnection_Click(object sender, EventArgs e)
    {
    }

    private void txtserial_TextChanged(object sender, EventArgs e)
    {
    }

    private void CheckUpdate_Click(object sender, EventArgs e)
    {
        try
        {
            CheckForUpdates();
        }
        catch (Exception ex)
        {
            Interaction.MsgBox(ex.ToString());
        }
    }

    private void Btn_Salir_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private void txtpass_KeyPress(System.Object sender, System.Windows.Forms.KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            try
            {
                conexion.Open();
                string Query;
                Query = string.Format("SELECT LOG_USER,LOG_PASS FROM Usuarios WHERE LOG_USER = '{0}' AND LOG_PASS = '{1}'", txtuser.Text.Trim(), txtpass.Text.Trim());
                cmd = new SqlCommand(Query, conexion);
                reader = cmd.ExecuteReader();

                int count;
                count = 0;
                while (reader.Read())
                    count = count + 1;

                if (count == 1)
                {
                    this.Hide();
                    Frm_Interfaz.Show();
                    conexion.Close();
                }
                else if (count > 1)
                {
                    Interaction.MsgBox("Usuario repetido!! Comuniquese con su proveedor!", MsgBoxStyle.Critical, "Error fatal");
                    conexion.Close();
                }
                else
                {
                    Interaction.MsgBox("Usuario y/o Contaseña incorrecta", MsgBoxStyle.Critical);
                    conexion.Close();
                    txtpass.Clear();
                    txtpass.Focus();
                }
                conexion.Close();
            }
            catch (SqlException ex)
            {
                Interaction.MsgBox(ex.ToString());
                txtuser.Clear();
                txtuser.Focus();
                txtpass.Visible = false;
            }
            finally
            {
                conexion.Close();
            }
        }
    }

    private void Button1_Click(object sender, EventArgs e)
    {
        try
        {
            conexion.Open();
            Interaction.MsgBox("holi");
        }
        catch (Exception ex)
        {
            Interaction.MsgBox(ex.ToString());
        }
    }

    private void txtuser_KeyPress(System.Object sender, System.Windows.Forms.KeyEventArgs e)
    {
        if (e.KeyCode == Keys.Enter)
        {
            txtpass.Focus();
            try
            {
                ds.Clear();
                conexion.Open();
                cmd = new SqlCommand("select LOG_USER from Usuarios where LOG_USER= '" + txtuser.Text + "'", conexion);
                da = new SqlDataAdapter(cmd);
                da.Fill(ds);

                reader = cmd.ExecuteReader();

                int count;
                count = 0;
                while (reader.Read())
                    count = count + 1;

                if (count == 1)
                {
                    reader.Close();
                    conexion.Close();
                    txtpass.Visible = true;
                    txtpass.Focus();
                }
                else if (count > 2)
                {
                    Interaction.MsgBox("Usuario repetido!! Comuniquese con su proveedor!", MsgBoxStyle.Critical, "Error fatal");
                    conexion.Close();
                    reader.Close();
                }
                else
                {
                    Interaction.MsgBox("Usuario y/o Contaseña incorrecta", MsgBoxStyle.Critical);
                    conexion.Close();
                    reader.Close();
                    txtpass.Visible = false;
                    txtuser.Clear();
                    txtuser.Focus();
                }
                conexion.Close();
            }
            catch (Exception ex)
            {
                Interaction.MsgBox(ex.ToString());
                txtuser.Clear();
                txtuser.Focus();
                txtpass.Visible = false;
            }
            finally
            {
                conexion.Close();
            }
        }
    }
}
