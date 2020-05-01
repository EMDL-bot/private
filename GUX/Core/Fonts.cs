using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Reflection;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace GUX.Core
{
    public class Fonts
    {

        public string FontAwesomeTTF { get; set; }

        public Font FontAwesome { get; set; }

        public float Size { get; set; }

        public FontStyle Style { get; set; }

        public Fonts(float size)
        {
            FontAwesomeTTF = Application.StartupPath + "\\fontawesome-webfont.ttf";
            Style = FontStyle.Regular;
            Size = size;
            Reload();
        }


        public void Reload()
        {
            PrivateFontCollection f = new PrivateFontCollection();
            f.AddFontFile(FontAwesomeTTF);
            FontAwesome = new Font(f.Families[0], Size, Style);
        }

        public void Reload(float newSize)
        {
            Size = newSize;
            Reload();
        }

        public void Reload(FontStyle newStyle)
        {
            Style = newStyle;
            Reload();
        }

        private void Reload(float newSize, FontStyle newStyle)
        {
            Size = newSize;
            Style = newStyle;
            Reload();
        }

        public string UnicodeToChar(string hex)
        {
            int code = int.Parse(hex, System.Globalization.NumberStyles.HexNumber);
            string unicodeString = char.ConvertFromUtf32(code);
            return unicodeString;
        }

        public class fa
        {
            private Fonts ff;
            public fa(Fonts f)
            {
                ff = f;
            }

            public string glass { get { return ff.UnicodeToChar("f000"); } }
            public string music { get { return ff.UnicodeToChar("f001"); } }
            public string search { get { return ff.UnicodeToChar("f002"); } }
            public string envelope_o { get { return ff.UnicodeToChar("f003"); } }
            public string heart { get { return ff.UnicodeToChar("f004"); } }
            public string star { get { return ff.UnicodeToChar("f005"); } }
            public string star_o { get { return ff.UnicodeToChar("f006"); } }
            public string user { get { return ff.UnicodeToChar("f007"); } }
            public string film { get { return ff.UnicodeToChar("f008"); } }
            public string th_large { get { return ff.UnicodeToChar("f009"); } }
            public string th { get { return ff.UnicodeToChar("f00a"); } }
            public string th_list { get { return ff.UnicodeToChar("f00b"); } }
            public string check { get { return ff.UnicodeToChar("f00c"); } }
            public string remove { get { return ff.UnicodeToChar("f00d"); } }
            public string close { get { return ff.UnicodeToChar("f00d"); } }
            public string times { get { return ff.UnicodeToChar("f00d"); } }
            public string search_plus { get { return ff.UnicodeToChar("f00e"); } }
            public string search_minus { get { return ff.UnicodeToChar("f010"); } }
            public string power_off { get { return ff.UnicodeToChar("f011"); } }
            public string signal { get { return ff.UnicodeToChar("f012"); } }
            public string gear { get { return ff.UnicodeToChar("f013"); } }
            public string cog { get { return ff.UnicodeToChar("f013"); } }
            public string trash_o { get { return ff.UnicodeToChar("f014"); } }
            public string home { get { return ff.UnicodeToChar("f015"); } }
            public string file_o { get { return ff.UnicodeToChar("f016"); } }
            public string clock_o { get { return ff.UnicodeToChar("f017"); } }
            public string road { get { return ff.UnicodeToChar("f018"); } }
            public string download { get { return ff.UnicodeToChar("f019"); } }
            public string arrow_circle_o_down { get { return ff.UnicodeToChar("f01a"); } }
            public string arrow_circle_o_up { get { return ff.UnicodeToChar("f01b"); } }
            public string inbox { get { return ff.UnicodeToChar("f01c"); } }
            public string play_circle_o { get { return ff.UnicodeToChar("f01d"); } }
            public string rotate_right { get { return ff.UnicodeToChar("f01e"); } }
            public string repeat { get { return ff.UnicodeToChar("f01e"); } }
            public string refresh { get { return ff.UnicodeToChar("f021"); } }
            public string list_alt { get { return ff.UnicodeToChar("f022"); } }
            public string @lock { get { return ff.UnicodeToChar("f023"); } }
            public string flag { get { return ff.UnicodeToChar("f024"); } }
            public string headphones { get { return ff.UnicodeToChar("f025"); } }
            public string volume_off { get { return ff.UnicodeToChar("f026"); } }
            public string volume_down { get { return ff.UnicodeToChar("f027"); } }
            public string volume_up { get { return ff.UnicodeToChar("f028"); } }
            public string qrcode { get { return ff.UnicodeToChar("f029"); } }
            public string barcode { get { return ff.UnicodeToChar("f02a"); } }
            public string tag { get { return ff.UnicodeToChar("f02b"); } }
            public string tags { get { return ff.UnicodeToChar("f02c"); } }
            public string book { get { return ff.UnicodeToChar("f02d"); } }
            public string bookmark { get { return ff.UnicodeToChar("f02e"); } }
            public string print { get { return ff.UnicodeToChar("f02f"); } }
            public string camera { get { return ff.UnicodeToChar("f030"); } }
            public string font { get { return ff.UnicodeToChar("f031"); } }
            public string bold { get { return ff.UnicodeToChar("f032"); } }
            public string italic { get { return ff.UnicodeToChar("f033"); } }
            public string text_height { get { return ff.UnicodeToChar("f034"); } }
            public string text_width { get { return ff.UnicodeToChar("f035"); } }
            public string align_left { get { return ff.UnicodeToChar("f036"); } }
            public string align_center { get { return ff.UnicodeToChar("f037"); } }
            public string align_right { get { return ff.UnicodeToChar("f038"); } }
            public string align_justify { get { return ff.UnicodeToChar("f039"); } }
            public string list { get { return ff.UnicodeToChar("f03a"); } }
            public string dedent { get { return ff.UnicodeToChar("f03b"); } }
            public string outdent { get { return ff.UnicodeToChar("f03b"); } }
            public string indent { get { return ff.UnicodeToChar("f03c"); } }
            public string video_camera { get { return ff.UnicodeToChar("f03d"); } }
            public string photo { get { return ff.UnicodeToChar("f03e"); } }
            public string image { get { return ff.UnicodeToChar("f03e"); } }
            public string picture_o { get { return ff.UnicodeToChar("f03e"); } }
            public string pencil { get { return ff.UnicodeToChar("f040"); } }
            public string map_marker { get { return ff.UnicodeToChar("f041"); } }
            public string adjust { get { return ff.UnicodeToChar("f042"); } }
            public string tint { get { return ff.UnicodeToChar("f043"); } }
            public string edit { get { return ff.UnicodeToChar("f044"); } }
            public string pencil_square_o { get { return ff.UnicodeToChar("f044"); } }
            public string share_square_o { get { return ff.UnicodeToChar("f045"); } }
            public string check_square_o { get { return ff.UnicodeToChar("f046"); } }
            public string arrows { get { return ff.UnicodeToChar("f047"); } }
            public string step_backward { get { return ff.UnicodeToChar("f048"); } }
            public string fast_backward { get { return ff.UnicodeToChar("f049"); } }
            public string backward { get { return ff.UnicodeToChar("f04a"); } }
            public string play { get { return ff.UnicodeToChar("f04b"); } }
            public string pause { get { return ff.UnicodeToChar("f04c"); } }
            public string stop { get { return ff.UnicodeToChar("f04d"); } }
            public string forward { get { return ff.UnicodeToChar("f04e"); } }
            public string fast_forward { get { return ff.UnicodeToChar("f050"); } }
            public string step_forward { get { return ff.UnicodeToChar("f051"); } }
            public string eject { get { return ff.UnicodeToChar("f052"); } }
            public string chevron_left { get { return ff.UnicodeToChar("f053"); } }
            public string chevron_right { get { return ff.UnicodeToChar("f054"); } }
            public string plus_circle { get { return ff.UnicodeToChar("f055"); } }
            public string minus_circle { get { return ff.UnicodeToChar("f056"); } }
            public string times_circle { get { return ff.UnicodeToChar("f057"); } }
            public string check_circle { get { return ff.UnicodeToChar("f058"); } }
            public string question_circle { get { return ff.UnicodeToChar("f059"); } }
            public string info_circle { get { return ff.UnicodeToChar("f05a"); } }
            public string crosshairs { get { return ff.UnicodeToChar("f05b"); } }
            public string times_circle_o { get { return ff.UnicodeToChar("f05c"); } }
            public string check_circle_o { get { return ff.UnicodeToChar("f05d"); } }
            public string ban { get { return ff.UnicodeToChar("f05e"); } }
            public string arrow_left { get { return ff.UnicodeToChar("f060"); } }
            public string arrow_right { get { return ff.UnicodeToChar("f061"); } }
            public string arrow_up { get { return ff.UnicodeToChar("f062"); } }
            public string arrow_down { get { return ff.UnicodeToChar("f063"); } }
            public string mail_forward { get { return ff.UnicodeToChar("f064"); } }
            public string share { get { return ff.UnicodeToChar("f064"); } }
            public string expand { get { return ff.UnicodeToChar("f065"); } }
            public string compress { get { return ff.UnicodeToChar("f066"); } }
            public string plus { get { return ff.UnicodeToChar("f067"); } }
            public string minus { get { return ff.UnicodeToChar("f068"); } }
            public string asterisk { get { return ff.UnicodeToChar("f069"); } }
            public string exclamation_circle { get { return ff.UnicodeToChar("f06a"); } }
            public string gift { get { return ff.UnicodeToChar("f06b"); } }
            public string leaf { get { return ff.UnicodeToChar("f06c"); } }
            public string fire { get { return ff.UnicodeToChar("f06d"); } }
            public string eye { get { return ff.UnicodeToChar("f06e"); } }
            public string eye_slash { get { return ff.UnicodeToChar("f070"); } }
            public string warning { get { return ff.UnicodeToChar("f071"); } }
            public string exclamation_triangle { get { return ff.UnicodeToChar("f071"); } }
            public string plane { get { return ff.UnicodeToChar("f072"); } }
            public string calendar { get { return ff.UnicodeToChar("f073"); } }
            public string random { get { return ff.UnicodeToChar("f074"); } }
            public string comment { get { return ff.UnicodeToChar("f075"); } }
            public string magnet { get { return ff.UnicodeToChar("f076"); } }
            public string chevron_up { get { return ff.UnicodeToChar("f077"); } }
            public string chevron_down { get { return ff.UnicodeToChar("f078"); } }
            public string retweet { get { return ff.UnicodeToChar("f079"); } }
            public string shopping_cart { get { return ff.UnicodeToChar("f07a"); } }
            public string folder { get { return ff.UnicodeToChar("f07b"); } }
            public string folder_open { get { return ff.UnicodeToChar("f07c"); } }
            public string arrows_v { get { return ff.UnicodeToChar("f07d"); } }
            public string arrows_h { get { return ff.UnicodeToChar("f07e"); } }
            public string bar_chart_o { get { return ff.UnicodeToChar("f080"); } }
            public string bar_chart { get { return ff.UnicodeToChar("f080"); } }
            public string twitter_square { get { return ff.UnicodeToChar("f081"); } }
            public string facebook_square { get { return ff.UnicodeToChar("f082"); } }
            public string camera_retro { get { return ff.UnicodeToChar("f083"); } }
            public string key { get { return ff.UnicodeToChar("f084"); } }
            public string gears { get { return ff.UnicodeToChar("f085"); } }
            public string cogs { get { return ff.UnicodeToChar("f085"); } }
            public string comments { get { return ff.UnicodeToChar("f086"); } }
            public string thumbs_o_up { get { return ff.UnicodeToChar("f087"); } }
            public string thumbs_o_down { get { return ff.UnicodeToChar("f088"); } }
            public string star_half { get { return ff.UnicodeToChar("f089"); } }
            public string heart_o { get { return ff.UnicodeToChar("f08a"); } }
            public string sign_out { get { return ff.UnicodeToChar("f08b"); } }
            public string linkedin_square { get { return ff.UnicodeToChar("f08c"); } }
            public string thumb_tack { get { return ff.UnicodeToChar("f08d"); } }
            public string external_link { get { return ff.UnicodeToChar("f08e"); } }
            public string sign_in { get { return ff.UnicodeToChar("f090"); } }
            public string trophy { get { return ff.UnicodeToChar("f091"); } }
            public string github_square { get { return ff.UnicodeToChar("f092"); } }
            public string upload { get { return ff.UnicodeToChar("f093"); } }
            public string lemon_o { get { return ff.UnicodeToChar("f094"); } }
            public string phone { get { return ff.UnicodeToChar("f095"); } }
            public string square_o { get { return ff.UnicodeToChar("f096"); } }
            public string bookmark_o { get { return ff.UnicodeToChar("f097"); } }
            public string phone_square { get { return ff.UnicodeToChar("f098"); } }
            public string twitter { get { return ff.UnicodeToChar("f099"); } }
            public string facebook_f { get { return ff.UnicodeToChar("f09a"); } }
            public string facebook { get { return ff.UnicodeToChar("f09a"); } }
            public string github { get { return ff.UnicodeToChar("f09b"); } }
            public string unlock { get { return ff.UnicodeToChar("f09c"); } }
            public string credit_card { get { return ff.UnicodeToChar("f09d"); } }
            public string feed { get { return ff.UnicodeToChar("f09e"); } }
            public string rss { get { return ff.UnicodeToChar("f09e"); } }
            public string hdd_o { get { return ff.UnicodeToChar("f0a0"); } }
            public string bullhorn { get { return ff.UnicodeToChar("f0a1"); } }
            public string bell { get { return ff.UnicodeToChar("f0f3"); } }
            public string certificate { get { return ff.UnicodeToChar("f0a3"); } }
            public string hand_o_right { get { return ff.UnicodeToChar("f0a4"); } }
            public string hand_o_left { get { return ff.UnicodeToChar("f0a5"); } }
            public string hand_o_up { get { return ff.UnicodeToChar("f0a6"); } }
            public string hand_o_down { get { return ff.UnicodeToChar("f0a7"); } }
            public string arrow_circle_left { get { return ff.UnicodeToChar("f0a8"); } }
            public string arrow_circle_right { get { return ff.UnicodeToChar("f0a9"); } }
            public string arrow_circle_up { get { return ff.UnicodeToChar("f0aa"); } }
            public string arrow_circle_down { get { return ff.UnicodeToChar("f0ab"); } }
            public string globe { get { return ff.UnicodeToChar("f0ac"); } }
            public string wrench { get { return ff.UnicodeToChar("f0ad"); } }
            public string tasks { get { return ff.UnicodeToChar("f0ae"); } }
            public string filter { get { return ff.UnicodeToChar("f0b0"); } }
            public string briefcase { get { return ff.UnicodeToChar("f0b1"); } }
            public string arrows_alt { get { return ff.UnicodeToChar("f0b2"); } }
            public string group { get { return ff.UnicodeToChar("f0c0"); } }
            public string users { get { return ff.UnicodeToChar("f0c0"); } }
            public string chain { get { return ff.UnicodeToChar("f0c1"); } }
            public string link { get { return ff.UnicodeToChar("f0c1"); } }
            public string cloud { get { return ff.UnicodeToChar("f0c2"); } }
            public string flask { get { return ff.UnicodeToChar("f0c3"); } }
            public string cut { get { return ff.UnicodeToChar("f0c4"); } }
            public string scissors { get { return ff.UnicodeToChar("f0c4"); } }
            public string copy { get { return ff.UnicodeToChar("f0c5"); } }
            public string files_o { get { return ff.UnicodeToChar("f0c5"); } }
            public string paperclip { get { return ff.UnicodeToChar("f0c6"); } }
            public string save { get { return ff.UnicodeToChar("f0c7"); } }
            public string floppy_o { get { return ff.UnicodeToChar("f0c7"); } }
            public string square { get { return ff.UnicodeToChar("f0c8"); } }
            public string navicon { get { return ff.UnicodeToChar("f0c9"); } }
            public string reorder { get { return ff.UnicodeToChar("f0c9"); } }
            public string bars { get { return ff.UnicodeToChar("f0c9"); } }
            public string list_ul { get { return ff.UnicodeToChar("f0ca"); } }
            public string list_ol { get { return ff.UnicodeToChar("f0cb"); } }
            public string strikethrough { get { return ff.UnicodeToChar("f0cc"); } }
            public string underline { get { return ff.UnicodeToChar("f0cd"); } }
            public string table { get { return ff.UnicodeToChar("f0ce"); } }
            public string magic { get { return ff.UnicodeToChar("f0d0"); } }
            public string truck { get { return ff.UnicodeToChar("f0d1"); } }
            public string pinterest { get { return ff.UnicodeToChar("f0d2"); } }
            public string pinterest_square { get { return ff.UnicodeToChar("f0d3"); } }
            public string google_plus_square { get { return ff.UnicodeToChar("f0d4"); } }
            public string google_plus { get { return ff.UnicodeToChar("f0d5"); } }
            public string money { get { return ff.UnicodeToChar("f0d6"); } }
            public string caret_down { get { return ff.UnicodeToChar("f0d7"); } }
            public string caret_up { get { return ff.UnicodeToChar("f0d8"); } }
            public string caret_left { get { return ff.UnicodeToChar("f0d9"); } }
            public string caret_right { get { return ff.UnicodeToChar("f0da"); } }
            public string columns { get { return ff.UnicodeToChar("f0db"); } }
            public string unsorted { get { return ff.UnicodeToChar("f0dc"); } }
            public string sort { get { return ff.UnicodeToChar("f0dc"); } }
            public string sort_down { get { return ff.UnicodeToChar("f0dd"); } }
            public string sort_desc { get { return ff.UnicodeToChar("f0dd"); } }
            public string sort_up { get { return ff.UnicodeToChar("f0de"); } }
            public string sort_asc { get { return ff.UnicodeToChar("f0de"); } }
            public string envelope { get { return ff.UnicodeToChar("f0e0"); } }
            public string linkedin { get { return ff.UnicodeToChar("f0e1"); } }
            public string rotate_left { get { return ff.UnicodeToChar("f0e2"); } }
            public string undo { get { return ff.UnicodeToChar("f0e2"); } }
            public string legal { get { return ff.UnicodeToChar("f0e3"); } }
            public string gavel { get { return ff.UnicodeToChar("f0e3"); } }
            public string dashboard { get { return ff.UnicodeToChar("f0e4"); } }
            public string tachometer { get { return ff.UnicodeToChar("f0e4"); } }
            public string comment_o { get { return ff.UnicodeToChar("f0e5"); } }
            public string comments_o { get { return ff.UnicodeToChar("f0e6"); } }
            public string flash { get { return ff.UnicodeToChar("f0e7"); } }
            public string bolt { get { return ff.UnicodeToChar("f0e7"); } }
            public string sitemap { get { return ff.UnicodeToChar("f0e8"); } }
            public string umbrella { get { return ff.UnicodeToChar("f0e9"); } }
            public string paste { get { return ff.UnicodeToChar("f0ea"); } }
            public string clipboard { get { return ff.UnicodeToChar("f0ea"); } }
            public string lightbulb_o { get { return ff.UnicodeToChar("f0eb"); } }
            public string exchange { get { return ff.UnicodeToChar("f0ec"); } }
            public string cloud_download { get { return ff.UnicodeToChar("f0ed"); } }
            public string cloud_upload { get { return ff.UnicodeToChar("f0ee"); } }
            public string user_md { get { return ff.UnicodeToChar("f0f0"); } }
            public string stethoscope { get { return ff.UnicodeToChar("f0f1"); } }
            public string suitcase { get { return ff.UnicodeToChar("f0f2"); } }
            public string bell_o { get { return ff.UnicodeToChar("f0a2"); } }
            public string coffee { get { return ff.UnicodeToChar("f0f4"); } }
            public string cutlery { get { return ff.UnicodeToChar("f0f5"); } }
            public string file_text_o { get { return ff.UnicodeToChar("f0f6"); } }
            public string building_o { get { return ff.UnicodeToChar("f0f7"); } }
            public string hospital_o { get { return ff.UnicodeToChar("f0f8"); } }
            public string ambulance { get { return ff.UnicodeToChar("f0f9"); } }
            public string medkit { get { return ff.UnicodeToChar("f0fa"); } }
            public string fighter_jet { get { return ff.UnicodeToChar("f0fb"); } }
            public string beer { get { return ff.UnicodeToChar("f0fc"); } }
            public string h_square { get { return ff.UnicodeToChar("f0fd"); } }
            public string plus_square { get { return ff.UnicodeToChar("f0fe"); } }
            public string angle_double_left { get { return ff.UnicodeToChar("f100"); } }
            public string angle_double_right { get { return ff.UnicodeToChar("f101"); } }
            public string angle_double_up { get { return ff.UnicodeToChar("f102"); } }
            public string angle_double_down { get { return ff.UnicodeToChar("f103"); } }
            public string angle_left { get { return ff.UnicodeToChar("f104"); } }
            public string angle_right { get { return ff.UnicodeToChar("f105"); } }
            public string angle_up { get { return ff.UnicodeToChar("f106"); } }
            public string angle_down { get { return ff.UnicodeToChar("f107"); } }
            public string desktop { get { return ff.UnicodeToChar("f108"); } }
            public string laptop { get { return ff.UnicodeToChar("f109"); } }
            public string tablet { get { return ff.UnicodeToChar("f10a"); } }
            public string mobile_phone { get { return ff.UnicodeToChar("f10b"); } }
            public string mobile { get { return ff.UnicodeToChar("f10b"); } }
            public string circle_o { get { return ff.UnicodeToChar("f10c"); } }
            public string quote_left { get { return ff.UnicodeToChar("f10d"); } }
            public string quote_right { get { return ff.UnicodeToChar("f10e"); } }
            public string spinner { get { return ff.UnicodeToChar("f110"); } }
            public string circle { get { return ff.UnicodeToChar("f111"); } }
            public string mail_reply { get { return ff.UnicodeToChar("f112"); } }
            public string reply { get { return ff.UnicodeToChar("f112"); } }
            public string github_alt { get { return ff.UnicodeToChar("f113"); } }
            public string folder_o { get { return ff.UnicodeToChar("f114"); } }
            public string folder_open_o { get { return ff.UnicodeToChar("f115"); } }
            public string smile_o { get { return ff.UnicodeToChar("f118"); } }
            public string frown_o { get { return ff.UnicodeToChar("f119"); } }
            public string meh_o { get { return ff.UnicodeToChar("f11a"); } }
            public string gamepad { get { return ff.UnicodeToChar("f11b"); } }
            public string keyboard_o { get { return ff.UnicodeToChar("f11c"); } }
            public string flag_o { get { return ff.UnicodeToChar("f11d"); } }
            public string flag_checkered { get { return ff.UnicodeToChar("f11e"); } }
            public string terminal { get { return ff.UnicodeToChar("f120"); } }
            public string code { get { return ff.UnicodeToChar("f121"); } }
            public string mail_reply_all { get { return ff.UnicodeToChar("f122"); } }
            public string reply_all { get { return ff.UnicodeToChar("f122"); } }
            public string star_half_empty { get { return ff.UnicodeToChar("f123"); } }
            public string star_half_full { get { return ff.UnicodeToChar("f123"); } }
            public string star_half_o { get { return ff.UnicodeToChar("f123"); } }
            public string location_arrow { get { return ff.UnicodeToChar("f124"); } }
            public string crop { get { return ff.UnicodeToChar("f125"); } }
            public string code_fork { get { return ff.UnicodeToChar("f126"); } }
            public string unlink { get { return ff.UnicodeToChar("f127"); } }
            public string chain_broken { get { return ff.UnicodeToChar("f127"); } }
            public string question { get { return ff.UnicodeToChar("f128"); } }
            public string info { get { return ff.UnicodeToChar("f129"); } }
            public string exclamation { get { return ff.UnicodeToChar("f12a"); } }
            public string superscript { get { return ff.UnicodeToChar("f12b"); } }
            public string subscript { get { return ff.UnicodeToChar("f12c"); } }
            public string eraser { get { return ff.UnicodeToChar("f12d"); } }
            public string puzzle_piece { get { return ff.UnicodeToChar("f12e"); } }
            public string microphone { get { return ff.UnicodeToChar("f130"); } }
            public string microphone_slash { get { return ff.UnicodeToChar("f131"); } }
            public string shield { get { return ff.UnicodeToChar("f132"); } }
            public string calendar_o { get { return ff.UnicodeToChar("f133"); } }
            public string fire_extinguisher { get { return ff.UnicodeToChar("f134"); } }
            public string rocket { get { return ff.UnicodeToChar("f135"); } }
            public string maxcdn { get { return ff.UnicodeToChar("f136"); } }
            public string chevron_circle_left { get { return ff.UnicodeToChar("f137"); } }
            public string chevron_circle_right { get { return ff.UnicodeToChar("f138"); } }
            public string chevron_circle_up { get { return ff.UnicodeToChar("f139"); } }
            public string chevron_circle_down { get { return ff.UnicodeToChar("f13a"); } }
            public string html5 { get { return ff.UnicodeToChar("f13b"); } }
            public string css3 { get { return ff.UnicodeToChar("f13c"); } }
            public string anchor { get { return ff.UnicodeToChar("f13d"); } }
            public string unlock_alt { get { return ff.UnicodeToChar("f13e"); } }
            public string bullseye { get { return ff.UnicodeToChar("f140"); } }
            public string ellipsis_h { get { return ff.UnicodeToChar("f141"); } }
            public string ellipsis_v { get { return ff.UnicodeToChar("f142"); } }
            public string rss_square { get { return ff.UnicodeToChar("f143"); } }
            public string play_circle { get { return ff.UnicodeToChar("f144"); } }
            public string ticket { get { return ff.UnicodeToChar("f145"); } }
            public string minus_square { get { return ff.UnicodeToChar("f146"); } }
            public string minus_square_o { get { return ff.UnicodeToChar("f147"); } }
            public string level_up { get { return ff.UnicodeToChar("f148"); } }
            public string level_down { get { return ff.UnicodeToChar("f149"); } }
            public string check_square { get { return ff.UnicodeToChar("f14a"); } }
            public string pencil_square { get { return ff.UnicodeToChar("f14b"); } }
            public string external_link_square { get { return ff.UnicodeToChar("f14c"); } }
            public string share_square { get { return ff.UnicodeToChar("f14d"); } }
            public string compass { get { return ff.UnicodeToChar("f14e"); } }
            public string toggle_down { get { return ff.UnicodeToChar("f150"); } }
            public string caret_square_o_down { get { return ff.UnicodeToChar("f150"); } }
            public string toggle_up { get { return ff.UnicodeToChar("f151"); } }
            public string caret_square_o_up { get { return ff.UnicodeToChar("f151"); } }
            public string toggle_right { get { return ff.UnicodeToChar("f152"); } }
            public string caret_square_o_right { get { return ff.UnicodeToChar("f152"); } }
            public string euro { get { return ff.UnicodeToChar("f153"); } }
            public string eur { get { return ff.UnicodeToChar("f153"); } }
            public string gbp { get { return ff.UnicodeToChar("f154"); } }
            public string dollar { get { return ff.UnicodeToChar("f155"); } }
            public string usd { get { return ff.UnicodeToChar("f155"); } }
            public string rupee { get { return ff.UnicodeToChar("f156"); } }
            public string inr { get { return ff.UnicodeToChar("f156"); } }
            public string cny { get { return ff.UnicodeToChar("f157"); } }
            public string rmb { get { return ff.UnicodeToChar("f157"); } }
            public string yen { get { return ff.UnicodeToChar("f157"); } }
            public string jpy { get { return ff.UnicodeToChar("f157"); } }
            public string ruble { get { return ff.UnicodeToChar("f158"); } }
            public string rouble { get { return ff.UnicodeToChar("f158"); } }
            public string rub { get { return ff.UnicodeToChar("f158"); } }
            public string won { get { return ff.UnicodeToChar("f159"); } }
            public string krw { get { return ff.UnicodeToChar("f159"); } }
            public string bitcoin { get { return ff.UnicodeToChar("f15a"); } }
            public string btc { get { return ff.UnicodeToChar("f15a"); } }
            public string file { get { return ff.UnicodeToChar("f15b"); } }
            public string file_text { get { return ff.UnicodeToChar("f15c"); } }
            public string sort_alpha_asc { get { return ff.UnicodeToChar("f15d"); } }
            public string sort_alpha_desc { get { return ff.UnicodeToChar("f15e"); } }
            public string sort_amount_asc { get { return ff.UnicodeToChar("f160"); } }
            public string sort_amount_desc { get { return ff.UnicodeToChar("f161"); } }
            public string sort_numeric_asc { get { return ff.UnicodeToChar("f162"); } }
            public string sort_numeric_desc { get { return ff.UnicodeToChar("f163"); } }
            public string thumbs_up { get { return ff.UnicodeToChar("f164"); } }
            public string thumbs_down { get { return ff.UnicodeToChar("f165"); } }
            public string youtube_square { get { return ff.UnicodeToChar("f166"); } }
            public string youtube { get { return ff.UnicodeToChar("f167"); } }
            public string xing { get { return ff.UnicodeToChar("f168"); } }
            public string xing_square { get { return ff.UnicodeToChar("f169"); } }
            public string youtube_play { get { return ff.UnicodeToChar("f16a"); } }
            public string dropbox { get { return ff.UnicodeToChar("f16b"); } }
            public string stack_overflow { get { return ff.UnicodeToChar("f16c"); } }
            public string instagram { get { return ff.UnicodeToChar("f16d"); } }
            public string flickr { get { return ff.UnicodeToChar("f16e"); } }
            public string adn { get { return ff.UnicodeToChar("f170"); } }
            public string bitbucket { get { return ff.UnicodeToChar("f171"); } }
            public string bitbucket_square { get { return ff.UnicodeToChar("f172"); } }
            public string tumblr { get { return ff.UnicodeToChar("f173"); } }
            public string tumblr_square { get { return ff.UnicodeToChar("f174"); } }
            public string long_arrow_down { get { return ff.UnicodeToChar("f175"); } }
            public string long_arrow_up { get { return ff.UnicodeToChar("f176"); } }
            public string long_arrow_left { get { return ff.UnicodeToChar("f177"); } }
            public string long_arrow_right { get { return ff.UnicodeToChar("f178"); } }
            public string apple { get { return ff.UnicodeToChar("f179"); } }
            public string windows { get { return ff.UnicodeToChar("f17a"); } }
            public string android { get { return ff.UnicodeToChar("f17b"); } }
            public string linux { get { return ff.UnicodeToChar("f17c"); } }
            public string dribbble { get { return ff.UnicodeToChar("f17d"); } }
            public string skype { get { return ff.UnicodeToChar("f17e"); } }
            public string foursquare { get { return ff.UnicodeToChar("f180"); } }
            public string trello { get { return ff.UnicodeToChar("f181"); } }
            public string female { get { return ff.UnicodeToChar("f182"); } }
            public string male { get { return ff.UnicodeToChar("f183"); } }
            public string gittip { get { return ff.UnicodeToChar("f184"); } }
            public string gratipay { get { return ff.UnicodeToChar("f184"); } }
            public string sun_o { get { return ff.UnicodeToChar("f185"); } }
            public string moon_o { get { return ff.UnicodeToChar("f186"); } }
            public string archive { get { return ff.UnicodeToChar("f187"); } }
            public string bug { get { return ff.UnicodeToChar("f188"); } }
            public string vk { get { return ff.UnicodeToChar("f189"); } }
            public string weibo { get { return ff.UnicodeToChar("f18a"); } }
            public string renren { get { return ff.UnicodeToChar("f18b"); } }
            public string pagelines { get { return ff.UnicodeToChar("f18c"); } }
            public string stack_exchange { get { return ff.UnicodeToChar("f18d"); } }
            public string arrow_circle_o_right { get { return ff.UnicodeToChar("f18e"); } }
            public string arrow_circle_o_left { get { return ff.UnicodeToChar("f190"); } }
            public string toggle_left { get { return ff.UnicodeToChar("f191"); } }
            public string caret_square_o_left { get { return ff.UnicodeToChar("f191"); } }
            public string dot_circle_o { get { return ff.UnicodeToChar("f192"); } }
            public string wheelchair { get { return ff.UnicodeToChar("f193"); } }
            public string vimeo_square { get { return ff.UnicodeToChar("f194"); } }
            public string turkish_lira { get { return ff.UnicodeToChar("f195"); } }
            public string @try { get { return ff.UnicodeToChar("f195"); } }
            public string plus_square_o { get { return ff.UnicodeToChar("f196"); } }
            public string space_shuttle { get { return ff.UnicodeToChar("f197"); } }
            public string slack { get { return ff.UnicodeToChar("f198"); } }
            public string envelope_square { get { return ff.UnicodeToChar("f199"); } }
            public string wordpress { get { return ff.UnicodeToChar("f19a"); } }
            public string openid { get { return ff.UnicodeToChar("f19b"); } }
            public string institution { get { return ff.UnicodeToChar("f19c"); } }
            public string bank { get { return ff.UnicodeToChar("f19c"); } }
            public string university { get { return ff.UnicodeToChar("f19c"); } }
            public string mortar_board { get { return ff.UnicodeToChar("f19d"); } }
            public string graduation_cap { get { return ff.UnicodeToChar("f19d"); } }
            public string yahoo { get { return ff.UnicodeToChar("f19e"); } }
            public string google { get { return ff.UnicodeToChar("f1a0"); } }
            public string reddit { get { return ff.UnicodeToChar("f1a1"); } }
            public string reddit_square { get { return ff.UnicodeToChar("f1a2"); } }
            public string stumbleupon_circle { get { return ff.UnicodeToChar("f1a3"); } }
            public string stumbleupon { get { return ff.UnicodeToChar("f1a4"); } }
            public string delicious { get { return ff.UnicodeToChar("f1a5"); } }
            public string digg { get { return ff.UnicodeToChar("f1a6"); } }
            public string pied_piper { get { return ff.UnicodeToChar("f1a7"); } }
            public string pied_piper_alt { get { return ff.UnicodeToChar("f1a8"); } }
            public string drupal { get { return ff.UnicodeToChar("f1a9"); } }
            public string joomla { get { return ff.UnicodeToChar("f1aa"); } }
            public string language { get { return ff.UnicodeToChar("f1ab"); } }
            public string fax { get { return ff.UnicodeToChar("f1ac"); } }
            public string building { get { return ff.UnicodeToChar("f1ad"); } }
            public string child { get { return ff.UnicodeToChar("f1ae"); } }
            public string paw { get { return ff.UnicodeToChar("f1b0"); } }
            public string spoon { get { return ff.UnicodeToChar("f1b1"); } }
            public string cube { get { return ff.UnicodeToChar("f1b2"); } }
            public string cubes { get { return ff.UnicodeToChar("f1b3"); } }
            public string behance { get { return ff.UnicodeToChar("f1b4"); } }
            public string behance_square { get { return ff.UnicodeToChar("f1b5"); } }
            public string steam { get { return ff.UnicodeToChar("f1b6"); } }
            public string steam_square { get { return ff.UnicodeToChar("f1b7"); } }
            public string recycle { get { return ff.UnicodeToChar("f1b8"); } }
            public string automobile { get { return ff.UnicodeToChar("f1b9"); } }
            public string car { get { return ff.UnicodeToChar("f1b9"); } }
            public string cab { get { return ff.UnicodeToChar("f1ba"); } }
            public string taxi { get { return ff.UnicodeToChar("f1ba"); } }
            public string tree { get { return ff.UnicodeToChar("f1bb"); } }
            public string spotify { get { return ff.UnicodeToChar("f1bc"); } }
            public string deviantart { get { return ff.UnicodeToChar("f1bd"); } }
            public string soundcloud { get { return ff.UnicodeToChar("f1be"); } }
            public string database { get { return ff.UnicodeToChar("f1c0"); } }
            public string file_pdf_o { get { return ff.UnicodeToChar("f1c1"); } }
            public string file_word_o { get { return ff.UnicodeToChar("f1c2"); } }
            public string file_excel_o { get { return ff.UnicodeToChar("f1c3"); } }
            public string file_powerpoint_o { get { return ff.UnicodeToChar("f1c4"); } }
            public string file_photo_o { get { return ff.UnicodeToChar("f1c5"); } }
            public string file_picture_o { get { return ff.UnicodeToChar("f1c5"); } }
            public string file_image_o { get { return ff.UnicodeToChar("f1c5"); } }
            public string file_zip_o { get { return ff.UnicodeToChar("f1c6"); } }
            public string file_archive_o { get { return ff.UnicodeToChar("f1c6"); } }
            public string file_sound_o { get { return ff.UnicodeToChar("f1c7"); } }
            public string file_audio_o { get { return ff.UnicodeToChar("f1c7"); } }
            public string file_movie_o { get { return ff.UnicodeToChar("f1c8"); } }
            public string file_video_o { get { return ff.UnicodeToChar("f1c8"); } }
            public string file_code_o { get { return ff.UnicodeToChar("f1c9"); } }
            public string vine { get { return ff.UnicodeToChar("f1ca"); } }
            public string codepen { get { return ff.UnicodeToChar("f1cb"); } }
            public string jsfiddle { get { return ff.UnicodeToChar("f1cc"); } }
            public string life_bouy { get { return ff.UnicodeToChar("f1cd"); } }
            public string life_buoy { get { return ff.UnicodeToChar("f1cd"); } }
            public string life_saver { get { return ff.UnicodeToChar("f1cd"); } }
            public string support { get { return ff.UnicodeToChar("f1cd"); } }
            public string life_ring { get { return ff.UnicodeToChar("f1cd"); } }
            public string circle_o_notch { get { return ff.UnicodeToChar("f1ce"); } }
            public string ra { get { return ff.UnicodeToChar("f1d0"); } }
            public string rebel { get { return ff.UnicodeToChar("f1d0"); } }
            public string ge { get { return ff.UnicodeToChar("f1d1"); } }
            public string empire { get { return ff.UnicodeToChar("f1d1"); } }
            public string git_square { get { return ff.UnicodeToChar("f1d2"); } }
            public string git { get { return ff.UnicodeToChar("f1d3"); } }
            public string y_combinator_square { get { return ff.UnicodeToChar("f1d4"); } }
            public string yc_square { get { return ff.UnicodeToChar("f1d4"); } }
            public string hacker_news { get { return ff.UnicodeToChar("f1d4"); } }
            public string tencent_weibo { get { return ff.UnicodeToChar("f1d5"); } }
            public string qq { get { return ff.UnicodeToChar("f1d6"); } }
            public string wechat { get { return ff.UnicodeToChar("f1d7"); } }
            public string weixin { get { return ff.UnicodeToChar("f1d7"); } }
            public string send { get { return ff.UnicodeToChar("f1d8"); } }
            public string paper_plane { get { return ff.UnicodeToChar("f1d8"); } }
            public string send_o { get { return ff.UnicodeToChar("f1d9"); } }
            public string paper_plane_o { get { return ff.UnicodeToChar("f1d9"); } }
            public string history { get { return ff.UnicodeToChar("f1da"); } }
            public string circle_thin { get { return ff.UnicodeToChar("f1db"); } }
            public string header { get { return ff.UnicodeToChar("f1dc"); } }
            public string paragraph { get { return ff.UnicodeToChar("f1dd"); } }
            public string sliders { get { return ff.UnicodeToChar("f1de"); } }
            public string share_alt { get { return ff.UnicodeToChar("f1e0"); } }
            public string share_alt_square { get { return ff.UnicodeToChar("f1e1"); } }
            public string bomb { get { return ff.UnicodeToChar("f1e2"); } }
            public string soccer_ball_o { get { return ff.UnicodeToChar("f1e3"); } }
            public string futbol_o { get { return ff.UnicodeToChar("f1e3"); } }
            public string tty { get { return ff.UnicodeToChar("f1e4"); } }
            public string binoculars { get { return ff.UnicodeToChar("f1e5"); } }
            public string plug { get { return ff.UnicodeToChar("f1e6"); } }
            public string slideshare { get { return ff.UnicodeToChar("f1e7"); } }
            public string twitch { get { return ff.UnicodeToChar("f1e8"); } }
            public string yelp { get { return ff.UnicodeToChar("f1e9"); } }
            public string newspaper_o { get { return ff.UnicodeToChar("f1ea"); } }
            public string wifi { get { return ff.UnicodeToChar("f1eb"); } }
            public string calculator { get { return ff.UnicodeToChar("f1ec"); } }
            public string paypal { get { return ff.UnicodeToChar("f1ed"); } }
            public string google_wallet { get { return ff.UnicodeToChar("f1ee"); } }
            public string cc_visa { get { return ff.UnicodeToChar("f1f0"); } }
            public string cc_mastercard { get { return ff.UnicodeToChar("f1f1"); } }
            public string cc_discover { get { return ff.UnicodeToChar("f1f2"); } }
            public string cc_amex { get { return ff.UnicodeToChar("f1f3"); } }
            public string cc_paypal { get { return ff.UnicodeToChar("f1f4"); } }
            public string cc_stripe { get { return ff.UnicodeToChar("f1f5"); } }
            public string bell_slash { get { return ff.UnicodeToChar("f1f6"); } }
            public string bell_slash_o { get { return ff.UnicodeToChar("f1f7"); } }
            public string trash { get { return ff.UnicodeToChar("f1f8"); } }
            public string copyright { get { return ff.UnicodeToChar("f1f9"); } }
            public string at { get { return ff.UnicodeToChar("f1fa"); } }
            public string eyedropper { get { return ff.UnicodeToChar("f1fb"); } }
            public string paint_brush { get { return ff.UnicodeToChar("f1fc"); } }
            public string birthday_cake { get { return ff.UnicodeToChar("f1fd"); } }
            public string area_chart { get { return ff.UnicodeToChar("f1fe"); } }
            public string pie_chart { get { return ff.UnicodeToChar("f200"); } }
            public string line_chart { get { return ff.UnicodeToChar("f201"); } }
            public string lastfm { get { return ff.UnicodeToChar("f202"); } }
            public string lastfm_square { get { return ff.UnicodeToChar("f203"); } }
            public string toggle_off { get { return ff.UnicodeToChar("f204"); } }
            public string toggle_on { get { return ff.UnicodeToChar("f205"); } }
            public string bicycle { get { return ff.UnicodeToChar("f206"); } }
            public string bus { get { return ff.UnicodeToChar("f207"); } }
            public string ioxhost { get { return ff.UnicodeToChar("f208"); } }
            public string angellist { get { return ff.UnicodeToChar("f209"); } }
            public string cc { get { return ff.UnicodeToChar("f20a"); } }
            public string shekel { get { return ff.UnicodeToChar("f20b"); } }
            public string sheqel { get { return ff.UnicodeToChar("f20b"); } }
            public string ils { get { return ff.UnicodeToChar("f20b"); } }
            public string meanpath { get { return ff.UnicodeToChar("f20c"); } }
            public string buysellads { get { return ff.UnicodeToChar("f20d"); } }
            public string connectdevelop { get { return ff.UnicodeToChar("f20e"); } }
            public string dashcube { get { return ff.UnicodeToChar("f210"); } }
            public string forumbee { get { return ff.UnicodeToChar("f211"); } }
            public string leanpub { get { return ff.UnicodeToChar("f212"); } }
            public string sellsy { get { return ff.UnicodeToChar("f213"); } }
            public string shirtsinbulk { get { return ff.UnicodeToChar("f214"); } }
            public string simplybuilt { get { return ff.UnicodeToChar("f215"); } }
            public string skyatlas { get { return ff.UnicodeToChar("f216"); } }
            public string cart_plus { get { return ff.UnicodeToChar("f217"); } }
            public string cart_arrow_down { get { return ff.UnicodeToChar("f218"); } }
            public string diamond { get { return ff.UnicodeToChar("f219"); } }
            public string ship { get { return ff.UnicodeToChar("f21a"); } }
            public string user_secret { get { return ff.UnicodeToChar("f21b"); } }
            public string motorcycle { get { return ff.UnicodeToChar("f21c"); } }
            public string street_view { get { return ff.UnicodeToChar("f21d"); } }
            public string heartbeat { get { return ff.UnicodeToChar("f21e"); } }
            public string venus { get { return ff.UnicodeToChar("f221"); } }
            public string mars { get { return ff.UnicodeToChar("f222"); } }
            public string mercury { get { return ff.UnicodeToChar("f223"); } }
            public string intersex { get { return ff.UnicodeToChar("f224"); } }
            public string transgender { get { return ff.UnicodeToChar("f224"); } }
            public string transgender_alt { get { return ff.UnicodeToChar("f225"); } }
            public string venus_double { get { return ff.UnicodeToChar("f226"); } }
            public string mars_double { get { return ff.UnicodeToChar("f227"); } }
            public string venus_mars { get { return ff.UnicodeToChar("f228"); } }
            public string mars_stroke { get { return ff.UnicodeToChar("f229"); } }
            public string mars_stroke_v { get { return ff.UnicodeToChar("f22a"); } }
            public string mars_stroke_h { get { return ff.UnicodeToChar("f22b"); } }
            public string neuter { get { return ff.UnicodeToChar("f22c"); } }
            public string genderless { get { return ff.UnicodeToChar("f22d"); } }
            public string facebook_official { get { return ff.UnicodeToChar("f230"); } }
            public string pinterest_p { get { return ff.UnicodeToChar("f231"); } }
            public string whatsapp { get { return ff.UnicodeToChar("f232"); } }
            public string server { get { return ff.UnicodeToChar("f233"); } }
            public string user_plus { get { return ff.UnicodeToChar("f234"); } }
            public string user_times { get { return ff.UnicodeToChar("f235"); } }
            public string hotel { get { return ff.UnicodeToChar("f236"); } }
            public string bed { get { return ff.UnicodeToChar("f236"); } }
            public string viacoin { get { return ff.UnicodeToChar("f237"); } }
            public string train { get { return ff.UnicodeToChar("f238"); } }
            public string subway { get { return ff.UnicodeToChar("f239"); } }
            public string medium { get { return ff.UnicodeToChar("f23a"); } }
            public string yc { get { return ff.UnicodeToChar("f23b"); } }
            public string y_combinator { get { return ff.UnicodeToChar("f23b"); } }
            public string optin_monster { get { return ff.UnicodeToChar("f23c"); } }
            public string opencart { get { return ff.UnicodeToChar("f23d"); } }
            public string expeditedssl { get { return ff.UnicodeToChar("f23e"); } }
            public string battery_4 { get { return ff.UnicodeToChar("f240"); } }
            public string battery_full { get { return ff.UnicodeToChar("f240"); } }
            public string battery_3 { get { return ff.UnicodeToChar("f241"); } }
            public string battery_three_quarters { get { return ff.UnicodeToChar("f241"); } }
            public string battery_2 { get { return ff.UnicodeToChar("f242"); } }
            public string battery_half { get { return ff.UnicodeToChar("f242"); } }
            public string battery_1 { get { return ff.UnicodeToChar("f243"); } }
            public string battery_quarter { get { return ff.UnicodeToChar("f243"); } }
            public string battery_0 { get { return ff.UnicodeToChar("f244"); } }
            public string battery_empty { get { return ff.UnicodeToChar("f244"); } }
            public string mouse_pointer { get { return ff.UnicodeToChar("f245"); } }
            public string i_cursor { get { return ff.UnicodeToChar("f246"); } }
            public string object_group { get { return ff.UnicodeToChar("f247"); } }
            public string object_ungroup { get { return ff.UnicodeToChar("f248"); } }
            public string sticky_note { get { return ff.UnicodeToChar("f249"); } }
            public string sticky_note_o { get { return ff.UnicodeToChar("f24a"); } }
            public string cc_jcb { get { return ff.UnicodeToChar("f24b"); } }
            public string cc_diners_club { get { return ff.UnicodeToChar("f24c"); } }
            public string clone { get { return ff.UnicodeToChar("f24d"); } }
            public string balance_scale { get { return ff.UnicodeToChar("f24e"); } }
            public string hourglass_o { get { return ff.UnicodeToChar("f250"); } }
            public string hourglass_1 { get { return ff.UnicodeToChar("f251"); } }
            public string hourglass_start { get { return ff.UnicodeToChar("f251"); } }
            public string hourglass_2 { get { return ff.UnicodeToChar("f252"); } }
            public string hourglass_half { get { return ff.UnicodeToChar("f252"); } }
            public string hourglass_3 { get { return ff.UnicodeToChar("f253"); } }
            public string hourglass_end { get { return ff.UnicodeToChar("f253"); } }
            public string hourglass { get { return ff.UnicodeToChar("f254"); } }
            public string hand_grab_o { get { return ff.UnicodeToChar("f255"); } }
            public string hand_rock_o { get { return ff.UnicodeToChar("f255"); } }
            public string hand_stop_o { get { return ff.UnicodeToChar("f256"); } }
            public string hand_paper_o { get { return ff.UnicodeToChar("f256"); } }
            public string hand_scissors_o { get { return ff.UnicodeToChar("f257"); } }
            public string hand_lizard_o { get { return ff.UnicodeToChar("f258"); } }
            public string hand_spock_o { get { return ff.UnicodeToChar("f259"); } }
            public string hand_pointer_o { get { return ff.UnicodeToChar("f25a"); } }
            public string hand_peace_o { get { return ff.UnicodeToChar("f25b"); } }
            public string trademark { get { return ff.UnicodeToChar("f25c"); } }
            public string registered { get { return ff.UnicodeToChar("f25d"); } }
            public string creative_commons { get { return ff.UnicodeToChar("f25e"); } }
            public string gg { get { return ff.UnicodeToChar("f260"); } }
            public string gg_circle { get { return ff.UnicodeToChar("f261"); } }
            public string tripadvisor { get { return ff.UnicodeToChar("f262"); } }
            public string odnoklassniki { get { return ff.UnicodeToChar("f263"); } }
            public string odnoklassniki_square { get { return ff.UnicodeToChar("f264"); } }
            public string get_pocket { get { return ff.UnicodeToChar("f265"); } }
            public string wikipedia_w { get { return ff.UnicodeToChar("f266"); } }
            public string safari { get { return ff.UnicodeToChar("f267"); } }
            public string chrome { get { return ff.UnicodeToChar("f268"); } }
            public string firefox { get { return ff.UnicodeToChar("f269"); } }
            public string opera { get { return ff.UnicodeToChar("f26a"); } }
            public string internet_explorer { get { return ff.UnicodeToChar("f26b"); } }
            public string tv { get { return ff.UnicodeToChar("f26c"); } }
            public string television { get { return ff.UnicodeToChar("f26c"); } }
            public string contao { get { return ff.UnicodeToChar("f26d"); } }
            public string _500px { get { return ff.UnicodeToChar("f26e"); } }
            public string amazon { get { return ff.UnicodeToChar("f270"); } }
            public string calendar_plus_o { get { return ff.UnicodeToChar("f271"); } }
            public string calendar_minus_o { get { return ff.UnicodeToChar("f272"); } }
            public string calendar_times_o { get { return ff.UnicodeToChar("f273"); } }
            public string calendar_check_o { get { return ff.UnicodeToChar("f274"); } }
            public string industry { get { return ff.UnicodeToChar("f275"); } }
            public string map_pin { get { return ff.UnicodeToChar("f276"); } }
            public string map_signs { get { return ff.UnicodeToChar("f277"); } }
            public string map_o { get { return ff.UnicodeToChar("f278"); } }
            public string map { get { return ff.UnicodeToChar("f279"); } }
            public string commenting { get { return ff.UnicodeToChar("f27a"); } }
            public string commenting_o { get { return ff.UnicodeToChar("f27b"); } }
            public string houzz { get { return ff.UnicodeToChar("f27c"); } }
            public string vimeo { get { return ff.UnicodeToChar("f27d"); } }
            public string black_tie { get { return ff.UnicodeToChar("f27e"); } }
            public string fonticons { get { return ff.UnicodeToChar("f280"); } }
        }
    }
}
