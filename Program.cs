using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using Discord;
using Discord.WebSocket;
using HtmlAgilityPack;

namespace DiscordBot
{
    public class Program
    {
        #region 변수모음
        int r = 1;
        string[] learn;
        decimal Timer;
        string Server = string.Empty;
        string Guild = string.Empty;
        string Class = string.Empty;
        string Title = string.Empty;
        string ItemLv = string.Empty;
        string ExpedLv = string.Empty;
        string PVP = string.Empty;
        string STAT, HP = string.Empty;
        string Critcal, Mastery, fast, patience, Overpower, skill;
        string chartacterCnt = string.Empty;
        string[] ItemList = new string[12];

        string imageurl = string.Empty;
        string imgclass = string.Empty;
        string[] imgNm;

        HtmlDocument doc = new HtmlDocument();
        HttpWebRequest request;
        HttpWebResponse response;
        StreamReader reader;

        string url = string.Empty;
        string strHtml = string.Empty;
        string itemlist_sub = string.Empty;
        string tmpText = string.Empty;
        string strHtml_sub = string.Empty;
        string strHtml_sub2 = string.Empty;
        string ArrayBoardList = string.Empty;
        string SubCharacter = string.Empty;

        string[] bbsNo = new string[50];
        string[] Subject = new string[50];
        string[] Link = new string[50];
        string[] SubjectDate = new string[50];
        string[] SubjectHit = new string[50];
        string[] SubjectReq = new string[50];
        string[] SubCharacters = new string[100];
        string[] Sub_Lv = new string[100];
        string[] SubInfo = new string[100];

        string Tmp;

        int cnt = 0;
        bool exchangeYN = false;
        bool masterYn = false;
        SocketGuild info;

        public string member { get; private set; }

        ulong userid;
        #endregion

        public static void Main(string[] args) => new Program().MainAsync().GetAwaiter().GetResult();

        public async Task MainAsync()
        {
            Process[] procs = Process.GetProcessesByName("DiscordBot_ver1");

            if (procs.Length > 1)
            {
                return;
            }
            else
            {
                DiscordSocketClient client = new DiscordSocketClient();
                client.Log += Log;
                string token = "NjU3NjIwNTkzNDc0MDExMTQ2.Xf12OA.E7BrzwqwiPDBUWg_PEO79hkfGHE";       //루페온 봇
                //string token = "NjU5MDAxMjkzMzcyOTE1NzI1.XgH9kw.qeSCwqMQDYQlUR2X8IBWEyA8WnQ";     //개발봇
                await client.LoginAsync(TokenType.Bot, token);
                await client.StartAsync();
                client.MessageReceived += MessageReceived;
                client.Ready += Client_Ready;
                client.GuildAvailable += Client_GuildAvailable;
                await client.SetGameAsync("?명령어 ", "", ActivityType.Streaming);
                await Task.Delay(-1); // 프로그램 종료시까지 태스크 유지      
            }  
        }

        private Task Client_Ready()
        {
            return Task.CompletedTask;
        }

        private Task Client_GuildAvailable(SocketGuild arg)
        {
            arg.DefaultChannel.SendMessageAsync("");
            //arg.GetTextChannel(657871364530634772).SendMessageAsync("**```cs\r\n봇이 '활성화' 되었습니다.\r\n```**");
            return Task.CompletedTask;
        }

        private async Task MessageReceived(SocketMessage message)
        {
            if (message.Content == "?명령어")
            {
                string Command = "**```마리상점검색 : ?마리\r\n" +
                                 "캐릭정보검색 : ?프로필 캐릭명\r\n" +
                                 "사사게  검색 : ?사사게 단어\r\n```**";


                masterYn = false;

                foreach (SocketRole role in ((SocketGuildUser)message.Author).Roles)
                {
                    //masterYn = role.Name == "관리자" ? true : false;
                    masterYn = role.Id == 557635038607573002 ? true : false;
                    if (masterYn)
                        break;
                }

                if (masterYn)
                {
                    string M_Command = Command.Replace("```**", "");
                    M_Command = M_Command + "봇 켜짐 알림 : /로그인\r\n봇 꺼짐 알림 : /로그아웃\r\n거래역할부여 : /거래인증 [디코이름#Tag] [캐릭명]\r\n```**";
                    await message.Channel.SendMessageAsync(embed: new EmbedBuilder { Title = "**봇 명령어모음**", Description = M_Command, Color = new Color(54, 57, 63) }.Build());
                }
                else
                {
                    await message.Channel.SendMessageAsync(embed: new EmbedBuilder { Title = "**봇 명령어모음**", Description = Command, Color = new Color(54, 57, 63) }.Build());
                }
            }

            if (message.Content.StartsWith("?프로필"))
            {
                switch (message.Channel.Id)
                {
                    case 657871364530634772:
                    case 658630760655355904:
                    case 657829380977852427:
                        #region 웹정보
                        doc = new HtmlDocument();

                        learn = message.Content.Split(' ');
                        url = "https://lostark.game.onstove.com/Profile/Character/" + learn[1];

                        doc.LoadHtml(url);

                        request = (HttpWebRequest)WebRequest.Create(url);
                        response = (HttpWebResponse)request.GetResponse();
                        reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                        strHtml = reader.ReadToEnd();

                        reader.Close();
                        response.Close();

                        strHtml_sub = strHtml.Substring(strHtml.IndexOf("main"));
                        #endregion

                        if (strHtml_sub.Contains("서버"))
                        {
                            #region 프로필
                            #region 서버
                            //서버
                            Server = strHtml_sub.Substring(strHtml_sub.IndexOf("서버"), 30);
                            Server = Server.Substring(Server.IndexOf("<span>"));
                            Server = Server.Replace("<span>", "").Replace("</span>", "");
                            Server = Server.Substring(0, Server.IndexOf("<")).Replace("@", "").Trim();
                            #endregion

                            #region 길드
                            //길드
                            Guild = strHtml_sub.Substring(strHtml_sub.IndexOf("길드"), 250);
                            Guild = Guild.Substring(Guild.IndexOf("<span>"));
                            Guild = Guild.Replace("<span>", "").Replace("</span>", "");
                            if (Guild.Contains("/guild.png"))
                            {
                                int g_end = Guild.IndexOf(">");
                                Guild = Guild.Substring(g_end + 1, 30);
                                int gg_end = Guild.IndexOf("<");
                                Guild = Guild.Substring(0, gg_end);
                            }
                            else
                            {
                                Guild = Guild.Substring(0, Guild.IndexOf("<")).Trim();
                            }
                            #endregion

                            #region 클래스
                            //클래스
                            Class = strHtml_sub.Substring(strHtml_sub.IndexOf("클래스"), 30);
                            Class = Class.Substring(Class.IndexOf("<span>"));
                            Class = Class.Replace("<span>", "").Replace("</span>", "");
                            Class = Class.Substring(0, Class.IndexOf("<")).Trim();
                            #endregion

                            #region 칭호
                            //칭호
                            Title = strHtml_sub.Substring(strHtml_sub.IndexOf("칭호"), 50);
                            Title = Title.Substring(Title.IndexOf("<span>"));
                            Title = Title.Replace("<span>", "").Replace("</span>", "");
                            int end = Title.IndexOf("</div>");
                            Title = Title.Substring(0, end).Trim();
                            #endregion

                            #region 아이템레벨
                            //템레벨
                            ItemLv = strHtml_sub.Substring(strHtml_sub.IndexOf("아이템"), 50);
                            ItemLv = ItemLv.Replace("아이템", "");
                            ItemLv = ItemLv.Replace("</span><span>", "").Replace("</span>", "");
                            ItemLv = ItemLv.Replace("<small>Lv.</small>", "").Replace("<small>", "");
                            ItemLv = ItemLv.Substring(0, ItemLv.IndexOf("<")).Replace(",", "").Trim();
                            #endregion

                            #region 원정대레벨
                            //원정대 레벨
                            ExpedLv = strHtml_sub.Substring(strHtml_sub.IndexOf("원정대"), 50);
                            ExpedLv = ExpedLv.Replace("원정대", "");
                            ExpedLv = ExpedLv.Replace("</span><span>", "").Replace("</span>", "");
                            ExpedLv = ExpedLv.Replace("<small>Lv.</small>", "").Replace("<small>", "");
                            ExpedLv = ExpedLv.Substring(0, ExpedLv.IndexOf("<")).Trim();
                            #endregion

                            #region 보유케릭
                            chartacterCnt = strHtml_sub.Substring(strHtml_sub.IndexOf("보유 캐릭터 "), 25);
                            chartacterCnt = chartacterCnt.Substring(chartacterCnt.IndexOf("<em>"));
                            chartacterCnt = chartacterCnt.Replace("<em>", "").Replace("</em>", "").Replace("</h3>", "").Replace("\r\n", "").Trim();
                            #endregion

                            #region 피브피
                            //PVP
                            PVP = strHtml_sub.Substring(strHtml_sub.IndexOf("PVP"), 30);
                            PVP = PVP.Substring(PVP.IndexOf("<span>"));
                            PVP = PVP.Replace("<span>", "").Replace("</span>", "");
                            PVP = PVP.Substring(0, PVP.IndexOf("<")).Trim();
                            #endregion

                            #region 힘,민,지
                            //능력치
                            STAT = strHtml_sub.Substring(strHtml_sub.IndexOf("기본 특성"), 80);
                            STAT = STAT.Substring(STAT.IndexOf("<span>"));
                            STAT = STAT.Replace("<span>", "").Replace("</span>", "").Replace("\r\n", "").Trim();
                            string[] tmp = STAT.Split(' ');
                            string tmpStat = tmp[0].Trim();
                            if (tmpStat.Length == 1)
                            {
                                tmpStat = tmpStat.PadRight(3, ' ');
                            }
                            else
                            {
                                tmpStat = tmpStat.PadRight(2, ' ');
                            }
                            string num_stat = tmp[1];
                            STAT = (tmpStat + " : #" + num_stat);
                            #endregion

                            #region 체력계수
                            //체력
                            HP = strHtml_sub.Substring(strHtml_sub.IndexOf(">체력</span>"), 30);
                            HP = HP.Substring(HP.IndexOf("<span>"));
                            HP = HP.Replace("</span", "").Replace("<span", "").Replace(">", "");
                            HP = HP.Replace("\r\n", "").Trim();
                            #endregion

                            #region 전투특성
                            //치명
                            Critcal = strHtml_sub.Substring(strHtml_sub.IndexOf(">치명<"), 30);
                            Critcal = Critcal.Substring(Critcal.IndexOf("<span>"));
                            Critcal = Critcal.Replace("</span>", "").Replace("<span>", "");
                            Critcal = Critcal.Replace("\r\n", "").Trim();

                            //특화
                            Mastery = strHtml_sub.Substring(strHtml_sub.IndexOf(">특화<"), 30);
                            Mastery = Mastery.Substring(Mastery.IndexOf("<span>"));
                            Mastery = Mastery.Replace("</span>", "").Replace("<span>", "");
                            Mastery = Mastery.Replace("\r\n", "").Trim();

                            //신속
                            fast = strHtml_sub.Substring(strHtml_sub.IndexOf(">신속<"), 30);
                            fast = fast.Substring(fast.IndexOf("<span>"));
                            fast = fast.Replace("</span>", "").Replace("<span>", "");
                            fast = fast.Replace("\r\n", "").Trim();

                            //제압
                            Overpower = strHtml_sub.Substring(strHtml_sub.IndexOf(">제압<"), 30);
                            Overpower = Overpower.Substring(Overpower.IndexOf("<span>"));
                            Overpower = Overpower.Replace("</span>", "").Replace("<span>", "");
                            Overpower = Overpower.Replace("\r\n", "").Trim();

                            //인내
                            patience = strHtml_sub.Substring(strHtml_sub.IndexOf(">인내<"), 30);
                            patience = patience.Substring(patience.IndexOf("<span>"));
                            patience = patience.Replace("</span>", "").Replace("<span>", "");
                            patience = patience.Replace("\r\n", "").Trim();

                            //숙련
                            skill = strHtml_sub.Substring(strHtml_sub.IndexOf(">숙련<"), 30);
                            skill = skill.Substring(skill.IndexOf("<span>"));
                            skill = skill.Replace("</span>", "").Replace("<span>", "");
                            skill = skill.Replace("\r\n", "").Trim();

                            #endregion

                            string _htmltext = strHtml_sub.Substring(strHtml_sub.IndexOf("보유 캐릭터 "));

                            string S_Server = string.Empty;

                            #region 부캐릭터정보
                            string S_tmp = string.Empty;

                            for (cnt = 0; cnt < Convert.ToInt32(chartacterCnt); cnt++)
                            {
                                if (cnt == 0)
                                {
                                    SubCharacters[cnt] = _htmltext;
                                    SubCharacters[cnt] = SubCharacters[cnt].Substring(SubCharacters[cnt].IndexOf("<li>"));
                                    SubCharacters[cnt] = SubCharacters[cnt].Substring(0, SubCharacters[cnt].IndexOf("</li>") + 6);
                                    S_tmp = SubCharacters[cnt];
                                    SubCharacters[cnt] = SubCharacters[cnt].Substring(SubCharacters[cnt].IndexOf("title"));
                                    SubCharacters[cnt] = SubCharacters[cnt].Substring(SubCharacters[0].IndexOf(">"));
                                    SubCharacters[cnt] = SubCharacters[cnt].Substring(1, (SubCharacters[0].IndexOf("</span>") - 1));
                                }
                                else
                                {
                                    SubCharacters[cnt] = _htmltext; //.Replace(SubCharacters[cnt - 1], "")
                                    SubCharacters[cnt] = SubCharacters[cnt].Substring(SubCharacters[cnt].IndexOf("<li>"));
                                    SubCharacters[cnt] = SubCharacters[cnt].Replace(S_tmp, "");
                                    SubCharacters[cnt] = SubCharacters[cnt].Substring(0, SubCharacters[cnt].IndexOf("</li>") + 6);
                                    S_tmp += SubCharacters[cnt];
                                    SubCharacters[cnt] = SubCharacters[cnt].Substring(SubCharacters[cnt].IndexOf("title"));
                                    SubCharacters[cnt] = SubCharacters[cnt].Substring(SubCharacters[cnt].IndexOf(">"));
                                    SubCharacters[cnt] = SubCharacters[cnt].Substring(1, SubCharacters[cnt].IndexOf("</span>") - 1);
                                }

                                url = "https://lostark.game.onstove.com/Profile/Character/" + SubCharacters[cnt];

                                doc.LoadHtml(url);

                                request = (HttpWebRequest)WebRequest.Create(url);
                                response = (HttpWebResponse)request.GetResponse();
                                reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                                strHtml = reader.ReadToEnd();

                                reader.Close();
                                reader.Dispose();

                                response.Close();
                                response.Dispose();

                                strHtml_sub2 = strHtml.Substring(strHtml.IndexOf("main"));

                                if (strHtml_sub.Contains("서버"))
                                {
                                    Sub_Lv[cnt] = strHtml_sub2.Substring(strHtml_sub2.IndexOf("아이템"), 50);
                                    Sub_Lv[cnt] = Sub_Lv[cnt].Replace("아이템", "");
                                    Sub_Lv[cnt] = Sub_Lv[cnt].Replace("</span><span>", "").Replace("</span>", "");
                                    Sub_Lv[cnt] = Sub_Lv[cnt].Replace("<small>Lv.</small>", "").Replace("<small>", "");
                                    Sub_Lv[cnt] = Sub_Lv[cnt].Substring(0, Sub_Lv[cnt].IndexOf("<")).Replace(",", "").Trim();

                                    S_Server = strHtml_sub2.Substring(strHtml_sub2.IndexOf("서버"), 30);
                                    S_Server = S_Server.Substring(S_Server.IndexOf("<span>"));
                                    S_Server = S_Server.Replace("<span>", "").Replace("</span>", "");
                                    S_Server = S_Server.Substring(0, S_Server.IndexOf("<")).Replace("@", "").Trim();

                                    SubInfo[cnt] = "**" + (cnt + 1) + ". 서버명 : " + S_Server + "\r\n Lv." + Sub_Lv[cnt] + " /  캐릭명 : " + SubCharacters[cnt] + "\r\n---------------------**";
                                }

                                if ((cnt + 1) == Convert.ToInt32(chartacterCnt))
                                {
                                    break;
                                }
                            }
                            #endregion 부캐릭터정보

                            string aa = "aa";
                            #endregion

                            for (int i = 0; i <= 5; i++)
                            {
                                Timer++;
                            }

                            Timer = 0;

                            #region 케릭이미지
                            ////케릭이미지     
                            //imageurl = strHtml_sub.Substring(strHtml_sub.IndexOf("equipment__character"));
                            //imageurl = imageurl.Substring(0, imageurl.IndexOf("</div>"));
                            //imageurl = imageurl.Substring(imageurl.IndexOf("cdn"));
                            //imageurl = imageurl.Substring(0, imageurl.IndexOf("alt")).Trim();
                            //imageurl = imageurl.Substring(0, imageurl.Length - 1);

                            //if (imageurl != null)
                            //{
                            //    imgNm = imageurl.Split('/');
                            //    WebRequest req = WebRequest.Create(new Uri("http://" + imageurl));
                            //    WebResponse result = req.GetResponse();
                            //    Stream str = result.GetResponseStream();

                            //    Byte[] read = new Byte[512];
                            //    int bytes = str.Read(read, 0, 512);
                            //    Encoding encode;
                            //    encode = System.Text.Encoding.Default;

                            //    FileStream FileStr = new FileStream("D:\\LostArk\\직업이미지\\" + imgNm[7], FileMode.OpenOrCreate, FileAccess.Write);

                            //    while (bytes > 0)
                            //    {
                            //        FileStr.Write(read, 0, bytes);
                            //        bytes = str.Read(read, 0, 512);
                            //    }
                            //    // Save File
                            //    BinaryWriter Savefile = new BinaryWriter(FileStr, encode);
                            //    Savefile.Close();

                            //    imgclass = "D:\\LostArk\\직업이미지\\" + imgNm[7];
                            //}
                            #endregion

                            string S_Info = string.Empty;

                            string msg = "**```css\r\n" +
                                         //"# [" + learn[1] + "] 님의 전투정보\r\n" +
                                         "! 캐릭터이름 : " + learn[1] + "\r\n" +
                                         "! 캐릭터서버 : " + Server + "  / 클　래　스 : " + Class + "\r\n" +
                                         "! 아이템레벨 : #" + ItemLv + " / 원정대레벨 : #" + ExpedLv + "\r\n" +
                                         "! 보유캐릭수 : #" + chartacterCnt + "\r\n\r\n" +
                                         "[기본정보]\r\n" +
                                         "! 길　드 : " + Guild + "\r\n" +
                                         "! 칭　호 : " + Title + "\r\n" +
                                         "! ＰＶＰ : " + PVP + "\r\n\r\n" +
                                         "[기본스텟]\r\n" +
                                         "! " + STAT + "\r\n" +
                                         "! 체력 : #" + HP + "\r\n\r\n" +
                                         "[전투특성]\r\n" +
                                         "! 치명 #" + Critcal.PadRight(4, ' ') + " / 특화 #" + Mastery.PadRight(4, ' ') + " / 제압 #" + Overpower.PadRight(4, ' ') + "\r\n" +
                                         "! 신속 #" + fast.PadRight(4, ' ') + " / 인내 #" + patience.PadRight(4, ' ') + " / 숙련 #" + skill.PadRight(4, ' ') + "\r\n\r\n" +
                                         "[참고사항]\r\n" +
                                         "! 전투특성이 인게임과 차이날 경우 펫효과가 적용안된 수치입니다.\r\n" +
                                         "! 캐릭터 선택 후 재접속시 적용된수치가 출력됩니다.```**";


                            #region MyRegion
                            //for (int s = 0; s <= SubInfo.Length; s++)
                            //{
                            //    if (SubInfo[s] == null)
                            //    {
                            //        break;
                            //    }
                            //    else
                            //    {
                            //        S_Info += SubInfo[s] + "\r\n";
                            //    }
                            //}

                            //masterYn = false;
                            //foreach (SocketRole role in ((SocketGuildUser)message.Author).Roles)
                            //{
                            //    masterYn = role.Id == 557635038607573002 ? true : false;
                            //    if (masterYn) break;
                            //}

                            //if (masterYn)
                            //{
                            //    await message.Channel.SendMessageAsync(embed: new EmbedBuilder { Title = "**[" + learn[1] + "] 님의 전투정보**", Description = msg, Color = new Color(54, 57, 63) }.Build());
                            //    await message.Channel.SendMessageAsync(embed: new EmbedBuilder { Title = "**[" + learn[1] + "] 님의 보유캐릭정보**", Description = S_Info, Color = new Color(54, 57, 63) }.Build());
                            //}
                            //else
                            //{
                            //    await message.Channel.SendMessageAsync(embed: new EmbedBuilder { Title = "**[" + learn[1] + "] 님의 전투정보**", Description = msg, Color = new Color(54, 57, 63) }.Build());
                            //}
                            #endregion

                            await message.Channel.SendMessageAsync(embed: new EmbedBuilder { Title = "**[" + learn[1] + "] 님의 전투정보**", Description = msg, Color = new Color(54, 57, 63) }.Build());
                        }
                        else
                        {
                            await message.Channel.SendMessageAsync("**```cs\r\n존재하지 않는 캐릭터명 입니다.\r\n```**");
                        }
                        break;
                    default:
                        await message.Channel.SendMessageAsync("**```cs\r\n" +
                                                           "# 프로필 검색은 전투정보실 채널을 이용해주세요.\r\n```** " +
                                                           "**``바로가기 ☞``**" + ((SocketGuildUser)message.Author).Guild.GetTextChannel(657871364530634772).Mention);
                        break;

                }
            }

            if (message.Content == "?마리")
            {
                #region 마리 
                #region 웹정보
                doc = new HtmlDocument();

                url = "https://lostark.game.onstove.com/Shop/Mari";

                doc.LoadHtml(url);

                request = (HttpWebRequest)WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), Encoding.Default);

                strHtml = reader.ReadToEnd();

                reader.Close();
                response.Close();

                itemlist_sub = strHtml.Substring(strHtml.IndexOf("listItems"));
                #endregion

                #region 항목
                ItemList[0] = itemlist_sub.Substring(itemlist_sub.IndexOf("-name"), 30);
                ItemList[0] = ItemList[0].Substring(ItemList[0].IndexOf(">")).Replace(">", "").Replace("<", "");
                ItemList[0] = ItemList[0].Substring(0, ItemList[0].IndexOf("/s"));
                #endregion 항목

                #region 가격
                ItemList[1] = itemlist_sub.Substring(itemlist_sub.IndexOf("currency"), 30);
                ItemList[1] = ItemList[1].Substring(ItemList[1].IndexOf(">") + 1).Replace("</span>", "").Replace("\r\n", "").Trim();
                #endregion 가격

                #region 항목2
                ItemList[2] = itemlist_sub.Substring(itemlist_sub.IndexOf("</li>"));
                tmpText = ItemList[2];

                ItemList[2] = ItemList[2].Substring(ItemList[2].IndexOf("-name"), 30);
                ItemList[2] = ItemList[2].Substring(ItemList[2].IndexOf(">")).Replace(">", "").Replace("<", "");
                ItemList[2] = ItemList[2].Substring(0, ItemList[2].IndexOf("/s"));
                #endregion

                #region 가격2
                ItemList[3] = tmpText.Substring(tmpText.IndexOf("currency"), 30);
                ItemList[3] = ItemList[3].Substring(ItemList[3].IndexOf(">") + 1).Replace("</span>", "").Replace("\r\n", "").Trim();
                #endregion

                #region 항목3
                tmpText = tmpText.Substring(tmpText.IndexOf("<li "));
                tmpText = tmpText.Substring(tmpText.IndexOf("</li>"));

                ItemList[4] = tmpText.Substring(tmpText.IndexOf("-name"), 30);
                ItemList[4] = ItemList[4].Substring(ItemList[4].IndexOf(">")).Replace(">", "").Replace("<", "");
                ItemList[4] = ItemList[4].Substring(0, ItemList[4].IndexOf("/s"));
                #endregion 

                #region 가격3
                ItemList[5] = tmpText.Substring(tmpText.IndexOf("currency"), 30);
                ItemList[5] = ItemList[5].Substring(ItemList[5].IndexOf(">") + 1).Replace("</span>", "").Replace("\r\n", "").Trim();
                #endregion

                #region 항목4
                tmpText = tmpText.Substring(tmpText.IndexOf("<li "));
                tmpText = tmpText.Substring(tmpText.IndexOf("</li>"));

                ItemList[6] = tmpText.Substring(tmpText.IndexOf("-name"), 30);
                ItemList[6] = ItemList[6].Substring(ItemList[6].IndexOf(">")).Replace(">", "").Replace("<", "");
                ItemList[6] = ItemList[6].Substring(0, ItemList[6].IndexOf("/s"));
                #endregion

                #region 가격4
                ItemList[7] = tmpText.Substring(tmpText.IndexOf("currency"), 30);
                ItemList[7] = ItemList[7].Substring(ItemList[7].IndexOf(">") + 1).Replace("</span>", "").Replace("\r\n", "").Trim();
                #endregion

                #region 항목5
                tmpText = tmpText.Substring(tmpText.IndexOf("<li "));
                tmpText = tmpText.Substring(tmpText.IndexOf("</li>"));

                ItemList[8] = tmpText.Substring(tmpText.IndexOf("-name"), 30);
                ItemList[8] = ItemList[8].Substring(ItemList[8].IndexOf(">")).Replace(">", "").Replace("<", "");
                ItemList[8] = ItemList[8].Substring(0, ItemList[8].IndexOf("/s"));
                #endregion 

                #region 가격5
                ItemList[9] = tmpText.Substring(tmpText.IndexOf("currency"), 30);
                ItemList[9] = ItemList[9].Substring(ItemList[9].IndexOf(">") + 1).Replace("</span>", "").Replace("\r\n", "").Trim();
                #endregion 

                #region 항목6
                tmpText = tmpText.Substring(tmpText.IndexOf("<li "));
                tmpText = tmpText.Substring(tmpText.IndexOf("</li>"));

                ItemList[10] = tmpText.Substring(tmpText.IndexOf("-name"), 30);
                ItemList[10] = ItemList[10].Substring(ItemList[10].IndexOf(">")).Replace(">", "").Replace("<", "");
                ItemList[10] = ItemList[10].Substring(0, ItemList[10].IndexOf("/s"));
                #endregion 

                #region 가격6
                ItemList[11] = tmpText.Substring(tmpText.IndexOf("currency"), 30);
                ItemList[11] = ItemList[11].Substring(ItemList[11].IndexOf(">") + 1).Replace("</span>", "").Replace("\r\n", "").Trim();
                #endregion
                #endregion 마리

                string MariShop = "```css\r\n" +
                                  //"# 현재시각 마리상점\r\n\r\n" +
                                  "# " + ItemList[0] + "\r\n" +
                                  "#Crystal : " + ItemList[1] + "개 \r\n\r\n" +
                                  "# " + ItemList[2] + "\r\n" +
                                  "#Crystal : " + ItemList[3] + "개 \r\n\r\n" +
                                  "# " + ItemList[4] + "\r\n" +
                                  "#Crystal : " + ItemList[5] + "개 \r\n\r\n" +
                                  "# " + ItemList[6] + "\r\n" +
                                  "#Crystal : " + ItemList[7] + "개 \r\n\r\n" +
                                  "# " + ItemList[8] + "\r\n" +
                                  "#Crystal : " + ItemList[9] + "개 \r\n\r\n" +
                                  "# " + ItemList[10] + "\r\n" +
                                  "#Crystal : " + ItemList[11] + "개 \r\n\r\n" +
                                  "```";

                await message.Channel.SendMessageAsync(embed: new EmbedBuilder { Title = "**현재시각 마리상점**", Description = MariShop, Color = new Color(54, 57, 63) }.Build());

            }

            if (message.Content.StartsWith("?사사게"))
            {
                learn = message.Content.Split(' ');

                doc = new HtmlDocument();

                learn = message.Content.Split(' ');
                url = "http://www.inven.co.kr/board/lostark/5355?name=subject&keyword=" + learn[1];

                doc.LoadHtml(url);

                request = (HttpWebRequest)WebRequest.Create(url);
                response = (HttpWebResponse)request.GetResponse();
                reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                strHtml = reader.ReadToEnd();

                reader.Close();
                response.Close();

                int str = strHtml.IndexOf("<tbody>");

                //strHtml_sub = strHtml.Substring(strHtml.IndexOf("<tbody>"));
                strHtml_sub = strHtml.Substring(str);
                strHtml_sub = strHtml_sub.Substring(0, strHtml_sub.IndexOf("</tbody>"));

                ArrayBoardList = strHtml_sub.Substring(strHtml_sub.IndexOf("이용규칙"));
                if (ArrayBoardList.Contains("<TD class='bbsNo'>"))
                {
                    //글번호
                    #region 글번호
                    ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("<TD class='bbsNo'>"));
                    for (int i = 0; i < bbsNo.Length; i++)
                    {
                        if (i == 0)
                        {
                            bbsNo[i] = ArrayBoardList.Substring(0, ArrayBoardList.IndexOf("</TR>"));
                            Tmp = bbsNo[i];
                            bbsNo[i] = bbsNo[i].Substring(0, ArrayBoardList.IndexOf("</TD>"));
                            bbsNo[i] = bbsNo[i].Replace("<TD class='bbsNo'>", "");
                        }
                        else
                        {
                            if (ArrayBoardList.Replace(Tmp, "").Contains("<TD class='bbsNo'>"))
                            {
                                ArrayBoardList = ArrayBoardList.Replace(Tmp, "");
                                ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("<TD class='bbsNo'>"));
                                bbsNo[i] = ArrayBoardList.Substring(0, ArrayBoardList.IndexOf("</TR>"));
                                Tmp = bbsNo[i];
                                bbsNo[i] = bbsNo[i].Substring(0, ArrayBoardList.IndexOf("</TD>"));
                                bbsNo[i] = bbsNo[i].Replace("<TD class='bbsNo'>", "");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    #endregion

                    #region 링크
                    ArrayBoardList = strHtml_sub.Substring(strHtml_sub.IndexOf("이용규칙"));
                    ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("<TD class='bbsNo'>"));
                    for (int i = 0; i < Link.Length; i++)
                    {
                        if (i == 0)
                        {
                            Link[i] = ArrayBoardList.Substring(0, ArrayBoardList.IndexOf("</TR>"));
                            Tmp = Link[i];
                            Link[i] = Link[i].Substring(Link[i].IndexOf("HREF="));
                            Link[i] = Link[i].Substring(Link[i].IndexOf("http"));
                            Link[i] = Link[i].Substring(0, Link[i].IndexOf(">")-1);
                        }
                        else
                        {
                            if (ArrayBoardList.Replace(Tmp, "").Contains("<TD class='bbsNo'>"))
                            {
                                ArrayBoardList = ArrayBoardList.Replace(Tmp, "");
                                ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("<TD class='bbsNo'>"));
                                Link[i] = ArrayBoardList.Substring(0, ArrayBoardList.IndexOf("</TR>"));
                                Tmp = Link[i];
                                Link[i] = Link[i].Substring(Link[i].IndexOf("HREF="));
                                Link[i] = Link[i].Substring(Link[i].IndexOf("http"));
                                Link[i] = Link[i].Substring(0, Link[i].IndexOf(">")-1);
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    #endregion

                    #region 글제목
                    ArrayBoardList = strHtml_sub.Substring(strHtml_sub.IndexOf("이용규칙"));
                    ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("<TD class='bbsNo'>"));
                    for (int i = 0; i < Subject.Length; i++)
                    {
                        if (i == 0)
                        {
                            Subject[i] = ArrayBoardList.Substring(0, ArrayBoardList.IndexOf("</TR>"));
                            Tmp = Subject[i];
                            Subject[i] = Subject[i].Substring(Subject[i].IndexOf("HREF="));
                            Subject[i] = Subject[i].Substring(Subject[i].IndexOf("http"));
                            Subject[i] = Subject[i].Substring(Subject[i].IndexOf(">"));
                            Subject[i] = Subject[i].Replace("&nbsp;", " ");
                            Subject[i] = Subject[i].Substring(0, Subject[i].IndexOf("</A>") - 1).Trim();

                        }
                        else
                        {
                            if (ArrayBoardList.Replace(Tmp, "").Contains("<TD class='bbsNo'>"))
                            {
                                ArrayBoardList = ArrayBoardList.Replace(Tmp, "");
                                ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("<TD class='bbsNo'>"));
                                Subject[i] = ArrayBoardList.Substring(0, ArrayBoardList.IndexOf("</TR>"));
                                Tmp = Subject[i];
                                Subject[i] = Subject[i].Substring(Subject[i].IndexOf("HREF="));
                                Subject[i] = Subject[i].Substring(Subject[i].IndexOf("http"));
                                Subject[i] = Subject[i].Substring(Subject[i].IndexOf(">"));
                                Subject[i] = Subject[i].Replace("&nbsp;", " ");
                                Subject[i] = Subject[i].Substring(0, Subject[i].IndexOf("</A>") - 1).Trim();
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    #endregion

                    #region 글쓴날짜
                    ArrayBoardList = strHtml_sub.Substring(strHtml_sub.IndexOf("이용규칙"));
                    ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("<TD class='bbsNo'>"));
                    for (int i = 0; i < SubjectDate.Length; i++)
                    {
                        if (i == 0)
                        {
                            SubjectDate[i] = ArrayBoardList.Substring(0, ArrayBoardList.IndexOf("</TR>"));
                            Tmp = SubjectDate[i];
                            SubjectDate[i] = SubjectDate[i].Substring(SubjectDate[i].IndexOf("'date'"));
                            SubjectDate[i] = SubjectDate[i].Replace("&nbsp;", " ");
                            SubjectDate[i] = SubjectDate[i].Substring(7, SubjectDate[i].IndexOf("</TD>") - 1).Trim();
                            SubjectDate[i] = SubjectDate[i].Replace("</TD>", "");
                        }
                        else
                        {
                            if (ArrayBoardList.Replace(Tmp, "").Contains("<TD class='bbsNo'>"))
                            {
                                ArrayBoardList = ArrayBoardList.Replace(Tmp, "");
                                ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("<TD class='bbsNo'>"));
                                SubjectDate[i] = ArrayBoardList.Substring(0, ArrayBoardList.IndexOf("</TR>"));
                                Tmp = SubjectDate[i];
                                SubjectDate[i] = SubjectDate[i].Substring(SubjectDate[i].IndexOf("'date'"));
                                SubjectDate[i] = SubjectDate[i].Replace("&nbsp;", " ");
                                SubjectDate[i] = SubjectDate[i].Substring(7, SubjectDate[i].IndexOf("</TD>") - 1).Trim();
                                SubjectDate[i] = SubjectDate[i].Replace("</TD>", "");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    #endregion

                    #region 글조회수
                    ArrayBoardList = strHtml_sub.Substring(strHtml_sub.IndexOf("이용규칙"));
                    ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("<TD class='bbsNo'>"));
                    for (int i = 0; i < SubjectHit.Length; i++)
                    {
                        if (i == 0)
                        {
                            SubjectHit[i] = ArrayBoardList.Substring(0, ArrayBoardList.IndexOf("</TR>"));
                            Tmp = SubjectHit[i];
                            SubjectHit[i] = SubjectHit[i].Substring(ArrayBoardList.IndexOf("'hit'"));
                            SubjectHit[i] = SubjectHit[i].Replace("&nbsp;", " ");
                            SubjectHit[i] = SubjectHit[i].Substring(6, SubjectHit[i].IndexOf("</TD>")).Trim();
                            SubjectHit[i] = SubjectHit[i].Replace("</TD>", "");
                        }
                        else
                        {
                            if (ArrayBoardList.Replace(Tmp, "").Contains("<TD class='bbsNo'>"))
                            {
                                ArrayBoardList = ArrayBoardList.Replace(Tmp, "");
                                ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("<TD class='bbsNo'>"));
                                SubjectHit[i] = ArrayBoardList.Substring(0, ArrayBoardList.IndexOf("</TR>"));
                                Tmp = SubjectHit[i];
                                SubjectHit[i] = SubjectHit[i].Substring(ArrayBoardList.IndexOf("'hit'"));
                                SubjectHit[i] = SubjectHit[i].Replace("&nbsp;", " ");
                                SubjectHit[i] = SubjectHit[i].Substring(6, SubjectHit[i].IndexOf("</TD>")).Trim();
                                SubjectHit[i] = SubjectHit[i].Replace("</TD>", "");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    #endregion

                    #region 추천수
                    ArrayBoardList = strHtml_sub.Substring(strHtml_sub.IndexOf("이용규칙"));
                    ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("<TD class='bbsNo'>"));
                    for (int i = 0; i < SubjectReq.Length; i++)
                    {
                        if (i == 0)
                        {
                            SubjectReq[i] = ArrayBoardList.Substring(0, ArrayBoardList.IndexOf("</TR>"));
                            Tmp = SubjectReq[i];
                            SubjectReq[i] = SubjectReq[i].Substring(ArrayBoardList.IndexOf("'req'"));
                            SubjectReq[i] = SubjectReq[i].Replace("&nbsp;", " ");
                            SubjectReq[i] = SubjectReq[i].Substring(6, SubjectReq[i].IndexOf("</TD>")).Trim();
                            SubjectReq[i] = SubjectReq[i].Replace("</TD>", "");
                        }
                        else
                        {
                            if (ArrayBoardList.Replace(Tmp, "").Contains("<TD class='bbsNo'>"))
                            {
                                ArrayBoardList = ArrayBoardList.Replace(Tmp, "");
                                ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("<TD class='bbsNo'>"));
                                SubjectReq[i] = ArrayBoardList.Substring(0, ArrayBoardList.IndexOf("</TR>"));
                                Tmp = SubjectReq[i];
                                SubjectReq[i] = SubjectReq[i].Substring(ArrayBoardList.IndexOf("'req'"));
                                SubjectReq[i] = SubjectReq[i].Replace("&nbsp;", " ");
                                SubjectReq[i] = SubjectReq[i].Substring(6, SubjectReq[i].IndexOf("</TD>")).Trim();
                                SubjectReq[i] = SubjectReq[i].Replace("</TD>", "");
                            }
                            else
                            {
                                break;
                            }
                        }
                    }
                    #endregion

                    string msg = "[1]: " + Link[0] + "\r\n" +
                                 "[2]: " + Link[1] + "\r\n" +
                                 "[3]: " + Link[2];

                    if (bbsNo.Length > 0)
                    {
                        await message.Channel.SendMessageAsync(embed: new EmbedBuilder { Title = "**사건사고게시판 [ " + learn[1] + " ] 검색결과\r\n**", Description = msg, Color = new Color(54, 57, 63) }.Build());


                    //await message.Channel.SendMessageAsync("**```css\r\n사건사고게시판 [ " + learn[1] + " ] 검색결과```**\r\n" +
                    //                                       "[1]: " + Link[0] + "\r\n" +
                    //                                       "[2]: " + Link[1] + "\r\n" +
                    //                                       "[3]: " + Link[2]);
                    //for (int aa = 0; aa < Link.Length; aa++)
                    //{
                    //    if(Link[aa] != null)
                    //    {
                    //        await message.Channel.SendMessageAsync("[링  크 " + (aa + 1) + "]: " + Link[aa] + "\r\n");
                    //                                             //"[링  크 " + (aa + 1) + "]: " + Link[aa] + "\r\n" +
                    //                                             //"[링  크 " + (aa + 1) + "]: " + Link[aa] + "\r\n");   
                    //    }
                    //}
                }
            }
                else
                {
                    //게시물이 존재하지 않습니다.
                    ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("</TR>"));
                    ArrayBoardList = ArrayBoardList.Substring(ArrayBoardList.IndexOf("ㆍ"));
                    ArrayBoardList = ArrayBoardList.Substring(0, ArrayBoardList.IndexOf("\n\t"));
                    await message.Channel.SendMessageAsync("**```cs\r\n" +
                                                           "# " + ArrayBoardList + "```**");
                }
            }

            if (message.Content == "/로그인")
            {
                string LogOut = "**```cs\r\n봇이 '활성화' 되었습니다.\r\n```**";

                await ((SocketGuildUser)message.Author).Guild.GetTextChannel(657871364530634772).SendMessageAsync(embed: new EmbedBuilder { Title = "** 봇상태알림 **", Description = LogOut, Color = new Color(54, 57, 63) }.Build());
            }

            if (message.Content == "/로그아웃")
            {
                string LogOut = "**```cs\r\n현재는 봇의 상태는 #오프라인 입니다.\r\n명령어로 검색하셔도 반응하지 않습니다.\r\n```**";

                await ((SocketGuildUser)message.Author).Guild.GetTextChannel(657871364530634772).SendMessageAsync(embed: new EmbedBuilder { Title = "** 봇상태알림 **", Description = LogOut, Color = new Color(54, 57, 63) }.Build());
            }

            if (message.Content.StartsWith("/거래인증"))
            {
                learn = message.Content.Split(' ');
                string time = DateTime.Now.Hour.ToString().PadLeft(2, '0') + " : " + DateTime.Now.Minute.ToString().PadLeft(2, '0');

                foreach (SocketGuildUser guildUser in ((SocketGuildUser)message.Author).Guild.Users)
                {
                    if (guildUser.Username + "#" + guildUser.DiscriminatorValue == learn[1])
                    {
                        userid = guildUser.Id;
                        break;
                    }
                }

                foreach (SocketRole role in ((SocketGuildUser)message.Author).Guild.GetUser(userid).Roles)
                {
                    exchangeYN = role.Name == "거래소" ? true : false;
                    if (exchangeYN) break;
                }

                if (exchangeYN)
                {
                    string msg = "**알림 - 인증실패\r\n" +
                                 "디스코드 닉 - " + learn[1] + "\r\n" +
                                 "인증시도 닉 - " + learn[2] + "\r\n" +
                                 "인증시도날짜 - " + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "\r\n" +
                                 "인증시도시간 - " + time + "\r\n" +
                                 "인증실패사유 - 중복신청**";

                    await ((SocketGuildUser)message.Author).Guild.GetTextChannel(660553865086763008).SendMessageAsync(embed: new EmbedBuilder { Description = msg, Color = new Color(54, 57, 63) }.Build());
                    return;
                }
                else
                {
                    #region 웹정보
                    doc = new HtmlDocument();

                    learn = message.Content.Split(' ');
                    url = "https://lostark.game.onstove.com/Profile/Character/" + learn[2];

                    doc.LoadHtml(url);

                    request = (HttpWebRequest)WebRequest.Create(url);
                    response = (HttpWebResponse)request.GetResponse();
                    reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
                    strHtml = reader.ReadToEnd();

                    reader.Close();
                    response.Close();

                    strHtml_sub = strHtml.Substring(strHtml.IndexOf("main"));
                    #endregion  웹정보

                    if (strHtml_sub.Contains("서버"))
                    {
                        #region 서버
                        //서버
                        Server = strHtml_sub.Substring(strHtml_sub.IndexOf("서버"), 30);
                        Server = Server.Substring(Server.IndexOf("<span>"));
                        Server = Server.Replace("<span>", "").Replace("</span>", "");
                        Server = Server.Substring(0, Server.IndexOf("<")).Replace("@", "").Trim();
                        #endregion

                        #region 아이템레벨
                        //템레벨
                        ItemLv = strHtml_sub.Substring(strHtml_sub.IndexOf("아이템"), 50);
                        ItemLv = ItemLv.Replace("아이템", "");
                        ItemLv = ItemLv.Replace("</span><span>", "").Replace("</span>", "");
                        ItemLv = ItemLv.Replace("<small>Lv.</small>", "").Replace("<small>", "");
                        ItemLv = ItemLv.Substring(0, ItemLv.IndexOf("<")).Replace(",", "").Trim();
                        #endregion

                        if (Server == "루페온" && Convert.ToDouble(ItemLv) > 725.00)
                        {
                            SocketRole n_role = ((SocketGuildUser)message.Author).Guild.GetRole(653263665574969386);
                            await ((SocketGuildUser)message.Author).Guild.GetUser(userid).AddRoleAsync(n_role);

                            string msg = "**알림 - 인증성공\r\n" +
                                         "디스코드 닉 - " + learn[1] + "\r\n" +
                                         "인증 닉넴 - " + learn[2] + "\r\n" +
                                         "인증 날짜 - " + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "\r\n" +
                                         "인증 시간 - " + time + "\r\n" +
                                         "역할 부여자 -" + message.Author + "**";

                            await message.Channel.SendMessageAsync("**```md\r\n# 역할부여완료.\r\n```**");
                            await ((SocketGuildUser)message.Author).Guild.GetTextChannel(660562755790831636).SendMessageAsync(embed: new EmbedBuilder { Description = msg, Color = new Color(54, 57, 63) }.Build());
                        }
                    }
                    else
                    {
                        string msg = "**알림 - 인증실패\r\n" +
                                     "디스코드 닉 - " + learn[1] + "\r\n" +
                                     "인증시도 닉 - " + learn[2] + "\r\n" +
                                     "인증시도날짜 - " + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "\r\n" +
                                     "인증시도시간 - " + time + "\r\n" +
                                     "인증실패사유 - 기준미달**";

                        await message.Channel.SendMessageAsync("**```cs\r\n# 역할부여실패.\r\n```**");
                        await ((SocketGuildUser)message.Author).Guild.GetTextChannel(660553865086763008).SendMessageAsync(embed: new EmbedBuilder { Description = msg, Color = new Color(54, 57, 63) }.Build());
                    }
                }
            }

            if (message.Content.StartsWith("?달력"))
            {
                if (message.Content.Contains(" "))
                {
                    learn = message.Content.Split(' ');
                    int Year = Convert.ToInt32(GetSplitString(learn[1], '/', 0));
                    int Month = Convert.ToInt32(GetSplitString(learn[1], '/', 1));
                    int Day = Convert.ToInt32(GetSplitString(learn[1], '/', 2));

                    DateTime dateTime = new DateTime(Year, Month, Day);

                    string Yoil = String.Empty;

                    switch (dateTime.DayOfWeek)
                    {
                        case DayOfWeek.Sunday:
                            Yoil = "일요일";
                            break;
                        case DayOfWeek.Monday:
                            Yoil = "월요일";
                            break;
                        case DayOfWeek.Tuesday:
                            Yoil = "화요일";
                            break;
                        case DayOfWeek.Wednesday:
                            Yoil = "로요일";
                            break;
                        case DayOfWeek.Thursday:
                            Yoil = "목요일";
                            break;
                        case DayOfWeek.Friday:
                            Yoil = "금요일";
                            break;
                        case DayOfWeek.Saturday:
                            Yoil = "토요일";
                            break;
                    }

                    await message.Channel.SendMessageAsync("**입력하신 " + Year + "년 " + Month + "월 " + Day + "일은 " + Yoil + "입니다.**");
                }
                else
                {
                    await message.Channel.SendMessageAsync("**원하는 날짜를 입력해주세요\r\nex) ?달력 2020/01/31**");
                }
            }

            #region 거래인증
            //if (message.Content.StartsWith("?거래인증"))
            //{
            //    switch (message.Channel.Id)
            //    {
            //        case 653482170425540618:
            //        case 658630760655355904:
            //        case 657829380977852427:
            //            #region 웹정보
            //            doc = new HtmlDocument();

            //            learn = message.Content.Split(' ');
            //            url = "https://lostark.game.onstove.com/Profile/Character/" + learn[1];

            //            doc.LoadHtml(url);

            //            request = (HttpWebRequest)WebRequest.Create(url);
            //            response = (HttpWebResponse)request.GetResponse();
            //            reader = new StreamReader(response.GetResponseStream(), Encoding.Default);
            //            strHtml = reader.ReadToEnd();

            //            reader.Close();
            //            response.Close();

            //            strHtml_sub = strHtml.Substring(strHtml.IndexOf("main"));
            //            #endregion

            //            if (strHtml_sub.Contains("서버"))
            //            {
            //                #region 서버
            //                //서버
            //                Server = strHtml_sub.Substring(strHtml_sub.IndexOf("서버"), 30);
            //                Server = Server.Substring(Server.IndexOf("<span>"));
            //                Server = Server.Replace("<span>", "").Replace("</span>", "");
            //                Server = Server.Substring(0, Server.IndexOf("<")).Replace("@", "").Trim();
            //                #endregion

            //                #region 아이템레벨
            //                //템레벨
            //                ItemLv = strHtml_sub.Substring(strHtml_sub.IndexOf("아이템"), 50);
            //                ItemLv = ItemLv.Replace("아이템", "");
            //                ItemLv = ItemLv.Replace("</span><span>", "").Replace("</span>", "");
            //                ItemLv = ItemLv.Replace("<small>Lv.</small>", "").Replace("<small>", "");
            //                ItemLv = ItemLv.Substring(0, ItemLv.IndexOf("<")).Replace(",", "").Trim();
            //                #endregion

            //                if (Convert.ToDouble(ItemLv) > 725.00)
            //                {
            //                    if (((SocketGuildUser)message.Author).Nickname == null)
            //                    {
            //                        member = ((SocketGuildUser)message.Author).Username;
            //                    }
            //                    else
            //                    {
            //                        member = ((SocketGuildUser)message.Author).Nickname;
            //                    }

            //                    if (member == learn[1] && Server == "루페온")
            //                    {
            //                        //내역할 찾기 
            //                        //역할명 :거래소  //역할ID :653263665574969386
            //                        foreach (SocketRole role in ((SocketGuildUser)message.Author).Roles)
            //                        {
            //                            exchangeYN = role.Name == "거래소" ? true : false;
            //                            if (exchangeYN) break;
            //                        }

            //                        if (exchangeYN)
            //                        {
            //                            await message.Channel.SendMessageAsync(null, false, embed: new EmbedBuilder { Description = "**```cs\r\n# 이미 해당역할을 부여받았습니다. 이중으로 부여받을 수 없습니다.```**", Color = new Color(54, 57, 63) }.Build());
            //                            return;
            //                        }
            //                        else
            //                        {
            //                            SocketRole n_role = ((SocketGuildUser)message.Author).Guild.GetRole(653263665574969386);
            //                            await ((IGuildUser)message.Author).AddRoleAsync(n_role);
            //                            await message.Channel.SendMessageAsync("**```cs\r\n" +
            //                                                                   "자격요건에 부합하여 '거래소역할' 을 부여해드렸습니다.\r\n```**\r\n" +
            //                                                                   "**```css\r\n" +
            //                                                                   "[ 안내사항 ]\r\n" +
            //                                                                   "# 해당역할은 디코내 거래소 이용을 위한 역할입니다. \r\n" +
            //                                                                   "# 역할이 있다고하여 100% 안전하지 않으니 유의하시기 바랍니다.\r\n" +
            //                                                                   "# 사칭 및 사기 등 거래 중 발생한 문제는 관리자가 대신 책임 지지 않습니다." +
            //                                                                   "```**");

            //                            string time = DateTime.Now.Hour.ToString().PadLeft(2, '0') + " : " + DateTime.Now.Minute.ToString().PadLeft(2, '0');

            //                            string msg = "**알림 - 인증성공\r\n" +
            //                                         "디스코드 닉 - " + message.Author + "\r\n" +
            //                                         "인증 닉넴 - " + learn[1] + "\r\n" +
            //                                         "인증 날짜 - " + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "\r\n" +
            //                                         "인증 시간 - " + time + "**";

            //                            await ((SocketGuildUser)message.Author).Guild.GetTextChannel(660562755790831636).SendMessageAsync(null, false, embed: new EmbedBuilder { Description = msg, Color = new Color(54, 57, 63) }.Build());
            //                        }
            //                    }
            //                    else
            //                    {
            //                        string time = DateTime.Now.Hour.ToString().PadLeft(2, '0') + " : " + DateTime.Now.Minute.ToString().PadLeft(2, '0');

            //                        string msg = "**알림 - 인증실패\r\n" +
            //                                     "디스코드 닉 - " + message.Author + "\r\n" +
            //                                     "인증시도 닉 - " + learn[1] + "\r\n" +
            //                                     "인증시도날짜 - " + DateTime.Now.Year + "/" + DateTime.Now.Month + "/" + DateTime.Now.Day + "\r\n" +
            //                                     "인증시도시간 - " + time + "**";

            //                        await ((SocketGuildUser)message.Author).Guild.GetTextChannel(660553865086763008).SendMessageAsync(null, false, embed: new EmbedBuilder { Description = msg, Color = new Color(54, 57, 63) }.Build());
            //                    }
            //                }
            //                else
            //                {
            //                    await message.Channel.SendMessageAsync("**```cs\r\n" +
            //                                                           "# 자격미달로 인하여 역할을 부여받을 수 없습니다.\r\n" +
            //                                                           "```**");
            //                }
            //            }
            //            else
            //            {
            //                await message.Channel.SendMessageAsync("**```cs\r\n존재하지 않는 캐릭터명 입니다.\r\n```**");
            //            }
            //            break;
            //        default:
            //            await message.Channel.SendMessageAsync("**```거래소 역할신청은 해당 게시판을 이용해주세요\r\n```** " +
            //                                               "**``바로가기 ☞``**" + ((SocketGuildUser)message.Author).Guild.GetTextChannel(653482170425540618).Mention);
            //            break;
            //    }
            //}
            #endregion
        }

        public string GetSplitString(string _txt, char _gubun, int _pos)
        {
            string[] SplitText = null;

            SplitText = _txt.Split(_gubun);

            for (int i = 0; i < SplitText.Length; i++)
            {
                if (i == _pos)
                {
                    break;
                }
            }

            return SplitText[_pos];
        }

        private Task Log(LogMessage msg)
        {
            Console.WriteLine(msg.ToString());
            return Task.CompletedTask;
        }
    }
}
