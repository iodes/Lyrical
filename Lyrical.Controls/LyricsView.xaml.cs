using System;
using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Animation;
using System.Windows.Controls;
using System.Windows.Threading;
using System.ComponentModel;
using Lyrical.Controls.Base;

namespace Lyrical.Controls
{
    /// <summary>
    /// LyricsView.xaml에 대한 상호 작용 논리
    /// </summary>
    public partial class LyricsView : UserControl
    {
        #region 속성
        /// <summary>
        /// 재생 시간 정보를 제공하는 객체입니다.
        /// </summary>
        public ILyricsProvider Provider { get; set; }

        /// <summary>
        /// 가사 정보를 담은 최상위 기본 객체입니다.
        /// </summary>
        public LyricsDocument Document { get; set; }
        #endregion

        #region 객체
        private LyricsSentence lastSentence;
        private LyricsCharacter lastLrCharacter;
        private LyricsCharacter lastPrCharacter;
        private LyricsCharacter lastTrCharacter;
        #endregion

        #region 생성자
        public LyricsView()
        {
            InitializeComponent();
            Loaded += LyricsView_Loaded;
        }
        #endregion

        #region 이벤트
        private void LyricsView_Loaded(object sender, RoutedEventArgs e)
        {
            if (!DesignerProperties.GetIsInDesignMode(this))
            {
                DispatcherTimer timer = new DispatcherTimer
                {
                    Interval = TimeSpan.FromMilliseconds(1)
                };

                timer.Tick += Timer_Tick;
                timer.Start();
            }
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            if (Document != null && Provider != null)
            {
                // 문장 및 문자 검색
                var sentence = Document.GetSentence(Provider.GetPosition());
                var lrCharacter = Document.GetCharacter(Provider.GetPosition(), LyricsDocument.ValueType.Lyrics);
                var prCharacter = Document.GetCharacter(Provider.GetPosition(), LyricsDocument.ValueType.Pronounce);
                var trCharacter = Document.GetCharacter(Provider.GetPosition(), LyricsDocument.ValueType.Translation);

                // 문장 블록 그리기
                if (lastSentence != sentence)
                {
                    if (lrCharacter != null)
                    {
                        CreateBlock(PanelLyrics, sentence.Characters.Lyrics);
                        TextLyrics.Visibility = Visibility.Collapsed;
                        PanelLyrics.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        TextLyrics.Text = sentence.Lyrics;
                        TextLyrics.Visibility = Visibility.Visible;
                        PanelLyrics.Visibility = Visibility.Collapsed;
                    }

                    if (prCharacter != null)
                    {
                        CreateBlock(PanelPronounce, sentence.Characters.Pronounce);
                        TextPronounce.Visibility = Visibility.Collapsed;
                        PanelPronounce.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        TextPronounce.Text = sentence.Pronounce;
                        TextPronounce.Visibility = Visibility.Visible;
                        PanelPronounce.Visibility = Visibility.Collapsed;
                    }

                    if (trCharacter != null)
                    {
                        CreateBlock(PanelTranslation, sentence.Characters.Translation);
                        TextTranslation.Visibility = Visibility.Collapsed;
                        PanelTranslation.Visibility = Visibility.Visible;
                    }
                    else
                    {
                        TextTranslation.Text = sentence.Translation;
                        TextTranslation.Visibility = Visibility.Visible;
                        PanelTranslation.Visibility = Visibility.Collapsed;
                    }

                    lastSentence = sentence;
                }

                // 애니메이션 적용
                if (lrCharacter != null)
                {
                    StartAnimation(ref lastLrCharacter, lrCharacter, PanelLyrics);
                }
                if (prCharacter != null)
                {
                    StartAnimation(ref lastPrCharacter, prCharacter, PanelPronounce);
                }
                if (trCharacter != null)
                {
                    StartAnimation(ref lastTrCharacter, trCharacter, PanelTranslation);
                }
            }
        }
        #endregion

        #region 내부 함수
        private void CreateBlock(StackPanel panel, LyricsCharacter[] value)
        {
            panel.Children.Clear();

            foreach (LyricsCharacter lrcChar in value)
            {
                panel.Children.Add(new TextBlock
                {
                    Text = lrcChar.Value,
                    FontSize = 22,
                    FontWeight = FontWeights.Bold,
                    Foreground = Brushes.White,
                    Opacity = 0,
                });
            }
        }

        private void StartAnimation(ref LyricsCharacter last, LyricsCharacter target, StackPanel panel)
        {
            if (last != target)
            {
                TextBlock currentBlock = panel.Children[target.Index] as TextBlock;

                Storyboard lyricsAnimation = new Storyboard();

                DoubleAnimation opacityAnimation = new DoubleAnimation
                {
                    From = 0,
                    To = 1,
                    Duration = target.EndTime - target.BeginTime,
                    EasingFunction = new CubicEase()
                };

                ThicknessAnimation marginAnimation = new ThicknessAnimation
                {
                    From = new Thickness(0, -20, 0, 0),
                    To = new Thickness(0),
                    Duration = target.EndTime - target.BeginTime,
                    EasingFunction = new CubicEase()
                };

                Storyboard.SetTargetProperty(opacityAnimation, new PropertyPath(OpacityProperty));
                Storyboard.SetTarget(opacityAnimation, currentBlock);

                Storyboard.SetTargetProperty(marginAnimation, new PropertyPath(MarginProperty));
                Storyboard.SetTarget(marginAnimation, currentBlock);

                lyricsAnimation.Children.Add(opacityAnimation);
                lyricsAnimation.Children.Add(marginAnimation);
                lyricsAnimation.Begin();

                last = target;
            }
        }
        #endregion
    }
}
