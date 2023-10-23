using ParserAvalonia.Models;
using ParserAvalonia.Services;
using System.Linq;
using System.Reactive;
using System.Threading;
using System.Threading.Tasks;
using ReactiveUI;

namespace ParserAvalonia.ViewModels;

public class MainViewModel : ViewModelBase
{
    #region Property
    
    private readonly FetchDataService _fetchDataService = new();
    private string _urlForSearch = "https://music.amazon.com/playlists/B08BWK8W15";
    private string _statusMsg = "";
    private Playlist _playlist;
    private double _listBoxOpacity;
    private double _playlistInfoOpacity;
    private double _statusMsgOpacity;
    private bool IsStatusMsgShowed = false;

    public string UrlForSearch { get => _urlForSearch; set => this.RaiseAndSetIfChanged(ref _urlForSearch, value); }
    public string StatusMsg { get => _statusMsg; set => this.RaiseAndSetIfChanged(ref _statusMsg, value); }
    public Playlist Playlist { get => _playlist; set => this.RaiseAndSetIfChanged(ref _playlist, value); }
    public double ListBoxOpacity { get => _listBoxOpacity; set => this.RaiseAndSetIfChanged(ref _listBoxOpacity, value); }
    public double PlaylistInfoOpacity { get => _playlistInfoOpacity; set => this.RaiseAndSetIfChanged(ref _playlistInfoOpacity, value); }
    public double StatusMsgOpacity { get => _statusMsgOpacity; set => this.RaiseAndSetIfChanged(ref _statusMsgOpacity, value); }
    public ReactiveCommand<Unit, Unit> SearchSongsBtn { get; }

    #endregion

    public MainViewModel()
    {
        SearchSongsBtn = ReactiveCommand.CreateFromTask(SearchSongsMthd);
    }


    private async Task SearchSongsMthd()
    {
        var cts = new CancellationTokenSource();
        var token = cts.Token;
        
        if(!IsStatusMsgShowed) await Task.Run(ShowStatusMsg);
        TypeWriterAnimationAsync("Start searching... Hold on pls.", token);
        
        var playlist = await Task.Run(() => GetPlaylist(UrlForSearch));
        if (playlist is null || !playlist.Songs.Any())
        {
            cts.Cancel();
            await TypeWriterAnimationAsync($"Nothing found by this url: {UrlForSearch}");
        }
        else
        {
            await Task.Run(ShowAnimation);
            Playlist = playlist;

            cts.Cancel();
            await TypeWriterAnimationAsync("You can check the result.");
        }

        cts.Dispose();
    }


    private async Task<Playlist> GetPlaylist(string url)
    {
        if (PlaylistInfoOpacity != 0.0 || ListBoxOpacity != 0.0)
        {
            Playlist = null;
            await Task.Run(HideAnimation);
        }

        return SongParserService.GetPlaylist(url);
    }


    private async Task ShowStatusMsg()
    {
        for (double i = 0.0; i < 1; i += 0.1)
        {
            StatusMsgOpacity = i;
            await Task.Delay(10);
        }
    }

    private async Task HideAnimation()
    {
        for (double i = 1; i > 0; i -= 0.1)
        {
            PlaylistInfoOpacity = i;
            ListBoxOpacity = i;
            await Task.Delay(10);
        }
    }

    private async Task ShowAnimation()
    {
        for (double i = 0.0; i < 1; i += 0.1)
        {
            PlaylistInfoOpacity = i;
            ListBoxOpacity = i;
            await Task.Delay(10);
        }
    }

    private async Task TypeWriterAnimationAsync(string text, CancellationToken token = default)
    {
        await Task.Run(async () =>
        {
            StatusMsg = "";
            for (int i = 0; i < text.Length; i++)
            {
                StatusMsg += text[i].ToString();
                await Task.Delay(50, token);
                
                if (token != default && i + 1 == text.Length)
                {
                    await Task.Delay(300, token);
                    StatusMsg = "";
                    i = -1;
                }
            }
        }, token);
    }
}
