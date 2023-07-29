using BoxClient.Game;
using System;
using System.Buffers.Binary;
using System.Diagnostics;
using System.Net;
using System.Net.Sockets;
using System.Reflection;

namespace BoxClient;

public partial class MainForm : Form
{
    private bool _fullscreen = false;
    private GameEngine _gameEngine = new();
    private bool _exit = false;
    private UdpClient _client;
    IPEndPoint _serverEndpoint;

    public MainForm(string server, int port)
    {
        InitializeComponent();

        _client = new UdpClient(server, port)
        {
            DontFragment = true
        };
        _client.AllowNatTraversal(true);
        _serverEndpoint = new IPEndPoint(IPAddress.Parse(server), port);
    }

    private void MainForm_Load(object sender, EventArgs e)
    {
        new Thread(() =>
        {
            var data = new byte[8];
            var lastUpdate = DateTime.Now.Ticks;
            while (!_exit)
            {
                BeginInvoke(() => Refresh());

                var now = DateTime.Now.Ticks;
                var delta = (now - lastUpdate) / (double)TimeSpan.TicksPerSecond;
                lastUpdate = now;

                _gameEngine.Update(delta);
                var x = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)(_gameEngine.Box.X * 1_000)));
                var y = BitConverter.GetBytes(IPAddress.HostToNetworkOrder((int)(_gameEngine.Box.Y * 1_000)));

                Buffer.BlockCopy(x, 0, data, 0, 4);
                Buffer.BlockCopy(y, 0, data, 4, 4);

                _client.Send(data, data.Length);

                if (_client.Available > 1)
                {
                    try
                    {
                        var receivedBytes = _client.Receive(ref _serverEndpoint);

                        if (receivedBytes.Length != 8)
                        {
                            throw new ApplicationException($"Received {receivedBytes.Length} bytes, expected 8.");
                        }

                        _gameEngine.ClearOthers();
                        var otherX = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(receivedBytes, 0)) / 1_000f;
                        var otherY = IPAddress.NetworkToHostOrder(BitConverter.ToInt32(receivedBytes, 4)) / 1_000f;

                        _gameEngine.AddOtherBox(new PointF(otherX, otherY));
                    }
                    catch (SocketException ex)
                    {
                        Debug.WriteLine("SocketException caught: {0}", ex.Message);
                    }
                }

                Thread.Sleep(20);
            }

            _client.Close();
        }).Start();
    }

    private void MainForm_KeyUp(object sender, KeyEventArgs e)
    {
        Debug.WriteLine($"KeyUp: {e.KeyCode}");

        if (e.Alt && e.KeyCode == Keys.Enter)
        {
            _fullscreen = !_fullscreen;
            GoToFullscreen(_fullscreen);
            e.Handled = true;
            return;
        }

        if (_gameEngine.KeyPress(e.KeyCode))
        {
            const bool off = false;
            if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W) _gameEngine.KeyUp = off;
            else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S) _gameEngine.KeyDown = off;
            else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A) _gameEngine.KeyLeft = off;
            else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D) _gameEngine.KeyRight = off;
        }
    }

    private void MainForm_KeyDown(object sender, KeyEventArgs e)
    {
        const bool on = true;
        if (e.KeyCode == Keys.Up || e.KeyCode == Keys.W)
        {
            _gameEngine.KeyUp = on;
            e.SuppressKeyPress = true;
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Down || e.KeyCode == Keys.S)
        {
            _gameEngine.KeyDown = on;
            e.SuppressKeyPress = true;
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Left || e.KeyCode == Keys.A)
        {
            _gameEngine.KeyLeft = on;
            e.SuppressKeyPress = true;
            e.Handled = true;
        }
        else if (e.KeyCode == Keys.Right || e.KeyCode == Keys.D)
        {
            _gameEngine.KeyRight = on;
            e.SuppressKeyPress = true;
            e.Handled = true;
        }
        else if (e.Alt && e.KeyCode == Keys.Enter)
        {
            e.SuppressKeyPress = true;
            e.Handled = true;
        }
    }

    private void MainForm_Activated(object sender, EventArgs e)
    {
        _gameEngine.IsRunning = true;
    }

    private void MainForm_Deactivate(object sender, EventArgs e)
    {
        _gameEngine.IsRunning = false;
    }

    private void MainForm_Paint(object sender, PaintEventArgs e)
    {
        _gameEngine.Draw(e.Graphics, e.ClipRectangle);
    }

    private void GoToFullscreen(bool fullscreen)
    {
        if (fullscreen)
        {
            FormBorderStyle = FormBorderStyle.None;
            WindowState = FormWindowState.Maximized;
        }
        else
        {
            FormBorderStyle = FormBorderStyle.Sizable;
            WindowState = FormWindowState.Normal;
        }
    }

    private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
    {
        _exit = true;
    }
}