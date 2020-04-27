import Alert from '@/components/Shared/Alert'
import DB_MC_PATIENT_INFO from '@/RCS_Data.Models.DB.DB_MC_PATIENT_INFO.json'
const hospList = [
  {
    hosp_desc: '0401180014國立台灣大學醫學院附設醫院本院',
    hosp_name: '國立台灣大學醫學院附設醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '120',
    location: [25.0402741, 121.5191591]
  },
  {
    hosp_desc: '0401180023國立臺灣大學醫學院附設醫院兒童醫院本院',
    hosp_name: '國立臺灣大學醫學院附設醫院兒童醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0442304, 121.5188189]
  },
  {
    hosp_desc: '0421040011國立成功大學醫學院附設醫院本院',
    hosp_name: '國立成功大學醫學院附設醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '75',
    location: [23.0021803, 120.2189867]
  },
  {
    hosp_desc: '0501110514三軍總醫院附設民眾診療服務處內湖院區',
    hosp_name: '三軍總醫院附設民眾診療服務處',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '45',
    location: [25.0718315, 121.5899564]
  },
  {
    hosp_desc: '0501110514三軍總醫院附設民眾診療服務處汀洲院區',
    hosp_name: '三軍總醫院附設民眾診療服務處',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '45',
    location: [25.0163971, 121.5297317]
  },
  {
    hosp_desc: '0601160016臺北榮民總醫院本院',
    hosp_name: '臺北榮民總醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '57',
    location: [25.1192105, 121.5203822]
  },
  {
    hosp_desc: '0602030026高雄榮民總醫院本院',
    hosp_name: '高雄榮民總醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '53',
    location: [22.6797767, 120.3223911]
  },
  {
    hosp_desc: '0617060018臺中榮民總醫院本院',
    hosp_name: '臺中榮民總醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '80',
    location: [24.1840398, 120.6049377]
  },
  {
    hosp_desc: '1101010012長庚醫療財團法人台北長庚紀念醫院本院',
    hosp_name: '長庚醫療財團法人台北長庚紀念醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '23',
    location: [25.0555992, 121.5496556]
  },
  {
    hosp_desc: '1101020018國泰醫療財團法人國泰綜合醫院本院',
    hosp_name: '國泰醫療財團法人國泰綜合醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '26',
    location: [25.0368806, 121.553594]
  },
  {
    hosp_desc: '1101100011台灣基督長老教會馬偕醫療財團法人馬偕紀念醫院本院',
    hosp_name: '台灣基督長老教會馬偕醫療財團法人馬偕紀念醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '39',
    location: [25.0588322, 121.5223675]
  },
  {
    hosp_desc: '1101100020台灣基督長老教會馬偕醫療財團法人馬偕兒童醫院本院',
    hosp_name: '台灣基督長老教會馬偕醫療財團法人馬偕兒童醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0588322, 121.5223675]
  },
  {
    hosp_desc: '1101150011新光醫療財團法人新光吳火獅紀念醫院本院',
    hosp_name: '新光醫療財團法人新光吳火獅紀念醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '40',
    location: [25.0962811, 121.5209845]
  },
  {
    hosp_desc: '1121020014奇美醫療財團法人奇美醫院台南分院本院',
    hosp_name: '奇美醫療財團法人奇美醫院台南分院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.9865344, 120.1961859]
  },
  {
    hosp_desc: '1131010011醫療財團法人徐元智先生醫藥基金會亞東紀念醫院本院',
    hosp_name: '醫療財團法人徐元智先生醫藥基金會亞東紀念醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '42',
    location: [24.9972647, 121.4527715]
  },
  {
    hosp_desc: '1131100010台灣基督長老教會馬偕醫療財團法人淡水馬偕紀念醫院本院',
    hosp_name: '台灣基督長老教會馬偕醫療財團法人淡水馬偕紀念醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '22',
    location: [25.1389266, 121.4618172]
  },
  {
    hosp_desc: '1132070011長庚醫療財團法人林口長庚紀念醫院本院',
    hosp_name: '長庚醫療財團法人林口長庚紀念醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '160',
    location: [25.06052, 121.368415]
  },
  {
    hosp_desc: '1137010024彰化基督教醫療財團法人彰化基督教醫院本院',
    hosp_name: '彰化基督教醫療財團法人彰化基督教醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '76',
    location: [24.0711417, 120.5446819]
  },
  {
    hosp_desc: '1137010042彰化基督教醫療財團法人彰化基督教兒童醫院本院',
    hosp_name: '彰化基督教醫療財團法人彰化基督教兒童醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '12',
    location: [24.0711154, 120.5436037]
  },
  {
    hosp_desc: '1141310019奇美醫療財團法人奇美醫院本院',
    hosp_name: '奇美醫療財團法人奇美醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '76',
    location: [23.0203921, 120.2218028]
  },
  {
    hosp_desc: '1141310019奇美醫療財團法人奇美醫院樹林院區',
    hosp_name: '奇美醫療財團法人奇美醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.9865344, 120.1961859]
  },
  {
    hosp_desc: '1142100017長庚醫療財團法人高雄長庚紀念醫院本院',
    hosp_name: '長庚醫療財團法人高雄長庚紀念醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '100',
    location: [22.6493809, 120.3527915]
  },
  {
    hosp_desc: '1145010010佛教慈濟醫療財團法人花蓮慈濟醫院本院',
    hosp_name: '佛教慈濟醫療財團法人花蓮慈濟醫院',
    hosp_class: '東區',
    hosp_city: '花蓮縣',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '36',
    location: [23.9962048, 121.5925472]
  },
  {
    hosp_desc: '1301200010臺北市立萬芳醫院－委託財團法人臺北醫學大學辦理本院',
    hosp_name: '臺北市立萬芳醫院－委託財團法人臺北醫學大學辦理',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '26',
    location: [24.999901, 121.55814]
  },
  {
    hosp_desc: '1302050014財團法人私立高雄醫學大學附設中和紀念醫院本院',
    hosp_name: '財團法人私立高雄醫學大學附設中和紀念醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '66',
    location: [22.6453863, 120.3102147]
  },
  {
    hosp_desc: '1303260014中國醫藥大學兒童醫院本院',
    hosp_name: '中國醫藥大學兒童醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '10',
    location: [24.157695, 120.680678]
  },
  {
    hosp_desc: '1317040011中山醫學大學附設醫院本院',
    hosp_name: '中山醫學大學附設醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '36',
    location: [24.1220819, 120.6515767]
  },
  {
    hosp_desc: '1317050017中國醫藥大學附設醫院本院',
    hosp_name: '中國醫藥大學附設醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '50',
    location: [24.1572247, 120.6804919]
  },
  {
    hosp_desc: '1131050515佛教慈濟醫療財團法人台北慈濟醫院本院',
    hosp_name: '佛教慈濟醫療財團法人台北慈濟醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '40',
    location: [24.9860629, 121.5356762]
  },
  {
    hosp_desc: '1142120001義大醫療財團法人義大醫院本院',
    hosp_name: '義大醫療財團法人義大醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '60',
    location: [22.7659067, 120.3643555]
  },
  {
    hosp_desc: '1331040513衛生福利部雙和醫院(委託臺北醫學大學興建經營)本院',
    hosp_name: '衛生福利部雙和醫院(委託臺北醫學大學興建經營)',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '34',
    location: [24.9926989, 121.4935259]
  },
  {
    hosp_desc: '0102080026高雄市立凱旋醫院本院',
    hosp_name: '高雄市立凱旋醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [22.6251221, 120.3240374]
  },
  {
    hosp_desc: '0131230012衛生福利部八里療養院療養院',
    hosp_name: '衛生福利部八里療養院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '3',
    location: [25.1446032, 121.4128086]
  },
  {
    hosp_desc: '0132010023衛生福利部桃園療養院療養院',
    hosp_name: '衛生福利部桃園療養院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '10',
    location: [24.9795463, 121.2688905]
  },
  {
    hosp_desc: '0138030010衛生福利部草屯療養院療養院',
    hosp_name: '衛生福利部草屯療養院',
    hosp_class: '中區',
    hosp_city: '南投縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.9898729, 120.7045675]
  },
  {
    hosp_desc: '0141270028衛生福利部嘉南療養院療養院',
    hosp_name: '衛生福利部嘉南療養院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [22.9766612, 120.2419539]
  },
  {
    hosp_desc: '1442060014財團法人台灣省私立高雄仁愛之家附設慈惠醫院本院',
    hosp_name: '財團法人台灣省私立高雄仁愛之家附設慈惠醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [22.6389567, 120.397688]
  },
  {
    hosp_desc: '0101090517臺北市立聯合醫院仁愛院區',
    hosp_name: '臺北市立聯合醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '15',
    location: [25.037512, 121.545224]
  },
  {
    hosp_desc: '0101090517臺北市立聯合醫院中興院區',
    hosp_name: '臺北市立聯合醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '10',
    location: [25.0509696, 121.5093209]
  },
  {
    hosp_desc: '0101090517臺北市立聯合醫院忠孝院區',
    hosp_name: '臺北市立聯合醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '12',
    location: [25.0466039, 121.5861594]
  },
  {
    hosp_desc: '0101090517臺北市立聯合醫院陽明院區',
    hosp_name: '臺北市立聯合醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '10',
    location: [25.1048315, 121.5314902]
  },
  {
    hosp_desc: '0101090517臺北市立聯合醫院和平院區',
    hosp_name: '臺北市立聯合醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '17',
    location: [25.0355293, 121.5066832]
  },
  {
    hosp_desc: '0101090517臺北市立聯合醫院婦幼院區',
    hosp_name: '臺北市立聯合醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '17',
    location: [25.029082, 121.5192681]
  },
  {
    hosp_desc: '0101090517臺北市立聯合醫院松德院區',
    hosp_name: '臺北市立聯合醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '16',
    location: [25.0305084, 121.5748954]
  },
  {
    hosp_desc: '0102020011高雄市立聯合醫院本院',
    hosp_name: '高雄市立聯合醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '20',
    location: [22.654927, 120.291194]
  },
  {
    hosp_desc:
      '0102070020高雄市立大同醫院(委託財團法人私立高雄醫學大學附設中和紀念醫院經營)本院',
    hosp_name:
      '高雄市立大同醫院(委託財團法人私立高雄醫學大學附設中和紀念醫院經營)',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '30',
    location: [22.6273601, 120.2974029]
  },
  {
    hosp_desc: '0111070010衛生福利部基隆醫院本院',
    hosp_name: '衛生福利部基隆醫院',
    hosp_class: '台北區',
    hosp_city: '基隆市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '12',
    location: [25.1302624, 121.7481291]
  },
  {
    hosp_desc: '0117030010衛生福利部臺中醫院本院',
    hosp_name: '衛生福利部臺中醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '6',
    location: [24.139099, 120.676502]
  },
  {
    hosp_desc: '0121050011衛生福利部臺南醫院本院',
    hosp_name: '衛生福利部臺南醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '10',
    location: [22.9973586, 120.2085726]
  },
  {
    hosp_desc: '0131020016新北市立聯合醫院本院',
    hosp_name: '新北市立聯合醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '12',
    location: [25.061166, 121.484419]
  },
  {
    hosp_desc: '0131020016新北市立聯合醫院板橋院區',
    hosp_name: '新北市立聯合醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '12',
    location: [25.0235828, 121.4576694]
  },
  {
    hosp_desc: '0131060029衛生福利部臺北醫院本院',
    hosp_name: '衛生福利部臺北醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '15',
    location: [25.0429644, 121.4595886]
  },
  {
    hosp_desc: '0132010014衛生福利部桃園醫院本院',
    hosp_name: '衛生福利部桃園醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '27',
    location: [24.9777031, 121.2691911]
  },
  {
    hosp_desc: '0135010016衛生福利部苗栗醫院本院',
    hosp_name: '衛生福利部苗栗醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '中度',
    hosp_ranking: '9',
    hosp_erbed: '10',
    location: [24.5755097, 120.8410317]
  },
  {
    hosp_desc: '0136010010衛生福利部豐原醫院本院',
    hosp_name: '衛生福利部豐原醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '20',
    location: [24.241983, 120.725672]
  },
  {
    hosp_desc: '0137170515衛生福利部彰化醫院本院',
    hosp_name: '衛生福利部彰化醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '5',
    location: [23.9506679, 120.5271781]
  },
  {
    hosp_desc: '0138010027衛生福利部南投醫院本院',
    hosp_name: '衛生福利部南投醫院',
    hosp_class: '中區',
    hosp_city: '南投縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '7',
    location: [23.9140079, 120.6849431]
  },
  {
    hosp_desc: '0138010027衛生福利部南投醫院中興院區',
    hosp_name: '衛生福利部南投醫院',
    hosp_class: '中區',
    hosp_city: '南投縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.952306, 120.6947558]
  },
  {
    hosp_desc: '0143010011衛生福利部屏東醫院本院',
    hosp_name: '衛生福利部屏東醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '8',
    location: [22.6733243, 120.4955525]
  },
  {
    hosp_desc: '0412040012國立臺灣大學醫學院附設醫院新竹分院本院',
    hosp_name: '國立臺灣大學醫學院附設醫院新竹分院',
    hosp_class: '北區',
    hosp_city: '新竹市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '30',
    location: [24.8152746, 120.9800043]
  },
  {
    hosp_desc: '0434010518國立陽明大學附設醫院本院',
    hosp_name: '國立陽明大學附設醫院',
    hosp_class: '台北區',
    hosp_city: '宜蘭縣',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '20',
    location: [24.7503103, 121.7587691]
  },
  {
    hosp_desc: '0434010518國立陽明大學附設醫院新民院區',
    hosp_name: '國立陽明大學附設醫院',
    hosp_class: '台北區',
    hosp_city: '宜蘭縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.7577719, 121.754158]
  },
  {
    hosp_desc: '0439010518國立臺灣大學醫學院附設醫院雲林分院本院',
    hosp_name: '國立臺灣大學醫學院附設醫院雲林分院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '28',
    location: [23.6975466, 120.5259195]
  },
  {
    hosp_desc: '0439010518國立臺灣大學醫學院附設醫院雲林分院虎尾院區',
    hosp_name: '國立臺灣大學醫學院附設醫院雲林分院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '6',
    location: [23.7350798, 120.4269473]
  },
  {
    hosp_desc: '0501010019三軍總醫院松山分院附設民眾診療服務處本院',
    hosp_name: '三軍總醫院松山分院附設民眾診療服務處',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '14',
    location: [25.054373, 121.557672]
  },
  {
    hosp_desc: '0501160014三軍總醫院北投分院附設民眾診療服務處本院',
    hosp_name: '三軍總醫院北投分院附設民眾診療服務處',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '6',
    location: [25.1395917, 121.5097658]
  },
  {
    hosp_desc: '0502030015國軍高雄總醫院左營分院附設民眾診療服務處本院',
    hosp_name: '國軍高雄總醫院左營分院附設民眾診療服務處',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '12',
    location: [22.7012121, 120.290507]
  },
  {
    hosp_desc: '0502080015國軍高雄總醫院附設民眾診療服務處本院',
    hosp_name: '國軍高雄總醫院附設民眾診療服務處',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '25',
    location: [22.6259888, 120.3419285]
  },
  {
    hosp_desc: '0532090029國軍桃園總醫院附設民眾診療服務處本院',
    hosp_name: '國軍桃園總醫院附設民眾診療服務處',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '20',
    location: [24.8769676, 121.2348227]
  },
  {
    hosp_desc: '0536190011國軍臺中總醫院附設民眾診療服務處本院',
    hosp_name: '國軍臺中總醫院附設民眾診療服務處',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '18',
    location: [24.150579, 120.7306598]
  },
  {
    hosp_desc: '0545040515國軍花蓮總醫院附設民眾診療服務處本院',
    hosp_name: '國軍花蓮總醫院附設民眾診療服務處',
    hosp_class: '東區',
    hosp_city: '花蓮縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '8',
    location: [24.0249411, 121.6064741]
  },
  {
    hosp_desc: '0622020017臺中榮民總醫院嘉義分院本院',
    hosp_name: '臺中榮民總醫院嘉義分院',
    hosp_class: '南區',
    hosp_city: '嘉義市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '5',
    location: [23.4674154, 120.422995]
  },
  {
    hosp_desc: '0632010014臺北榮民總醫院桃園分院本院',
    hosp_name: '臺北榮民總醫院桃園分院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '20',
    location: [25.0043796, 121.325243]
  },
  {
    hosp_desc: '0902080013阮綜合醫療社團法人阮綜合醫院本院',
    hosp_name: '阮綜合醫療社團法人阮綜合醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '30',
    location: [22.6154782, 120.2973496]
  },
  {
    hosp_desc: '0905320023台南市立醫院(委託秀傳醫療社團法人經營)本院',
    hosp_name: '台南市立醫院(委託秀傳醫療社團法人經營)',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '25',
    location: [22.9687059, 120.2268128]
  },
  {
    hosp_desc: '0917070029林新醫療社團法人林新醫院本院',
    hosp_name: '林新醫療社團法人林新醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '18',
    location: [24.1490126, 120.6403479]
  },
  {
    hosp_desc: '0932020025天成醫療社團法人天晟醫院本院',
    hosp_name: '天成醫療社團法人天晟醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '15',
    location: [24.9626431, 121.2290242]
  },
  {
    hosp_desc: '0933050018東元醫療社團法人東元綜合醫院本院',
    hosp_name: '東元醫療社團法人東元綜合醫院',
    hosp_class: '北區',
    hosp_city: '新竹縣',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '15',
    location: [24.82365, 121.013754]
  },
  {
    hosp_desc: '0936030018李綜合醫療社團法人大甲李綜合醫院本院',
    hosp_name: '李綜合醫療社團法人大甲李綜合醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '6',
    location: [24.3508226, 120.6180489]
  },
  {
    hosp_desc: '0936050029光田醫療社團法人光田綜合醫院沙鹿總院',
    hosp_name: '光田醫療社團法人光田綜合醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '12',
    location: [24.2354394, 120.5588678]
  },
  {
    hosp_desc: '0936050029光田醫療社團法人光田綜合醫院大甲院區',
    hosp_name: '光田醫療社團法人光田綜合醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '9',
    location: [24.3466462, 120.6164936]
  },
  {
    hosp_desc: '0936060016童綜合醫療社團法人童綜合醫院梧棲院區',
    hosp_name: '童綜合醫療社團法人童綜合醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '30',
    location: [24.2469991, 120.5423752]
  },
  {
    hosp_desc: '0936060016童綜合醫療社團法人童綜合醫院沙鹿院區',
    hosp_name: '童綜合醫療社團法人童綜合醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.242947, 120.56069]
  },
  {
    hosp_desc: '0937010019秀傳醫療社團法人秀傳紀念醫院本院',
    hosp_name: '秀傳醫療社團法人秀傳紀念醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '20',
    location: [24.064752, 120.536679]
  },
  {
    hosp_desc: '0943010017寶建醫療社團法人寶建醫院本院',
    hosp_name: '寶建醫療社團法人寶建醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '20',
    location: [22.6814439, 120.4843319]
  },
  {
    hosp_desc: '0943030019安泰醫療社團法人安泰醫院本院',
    hosp_name: '安泰醫療社團法人安泰醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '8',
    location: [22.4740818, 120.4592153]
  },
  {
    hosp_desc: '1101010021基督復臨安息日會醫療財團法人臺安醫院本院',
    hosp_name: '基督復臨安息日會醫療財團法人臺安醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '10',
    location: [25.0480541, 121.547339]
  },
  {
    hosp_desc: '1101160017振興醫療財團法人振興醫院本院',
    hosp_name: '振興醫療財團法人振興醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '25',
    location: [25.117512, 121.5224672]
  },
  {
    hosp_desc: '1101160026醫療財團法人辜公亮基金會和信治癌中心醫院本院',
    hosp_name: '醫療財團法人辜公亮基金會和信治癌中心醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '6',
    location: [25.1284955, 121.4720103]
  },
  {
    hosp_desc:
      '1102110011高雄市立小港醫院(委託財團法人私立高雄醫學大學經營)本院',
    hosp_name: '高雄市立小港醫院(委託財團法人私立高雄醫學大學經營)',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '30',
    location: [22.5674103, 120.3633173]
  },
  {
    hosp_desc: '1105040016台灣基督長老教會新樓醫療財團法人麻豆新樓醫院本院',
    hosp_name: '台灣基督長老教會新樓醫療財團法人麻豆新樓醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '10',
    location: [23.1802047, 120.2327989]
  },
  {
    hosp_desc: '1111060015長庚醫療財團法人基隆長庚紀念醫院本院',
    hosp_name: '長庚醫療財團法人基隆長庚紀念醫院',
    hosp_class: '台北區',
    hosp_city: '基隆市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '30',
    location: [25.1211061, 121.7223372]
  },
  {
    hosp_desc: '1111060015長庚醫療財團法人基隆長庚紀念醫院情人湖院區',
    hosp_name: '長庚醫療財團法人基隆長庚紀念醫院',
    hosp_class: '台北區',
    hosp_city: '基隆市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.1489038, 121.7078628]
  },
  {
    hosp_desc: '1112010519台灣基督長老教會馬偕醫療財團法人新竹馬偕紀念醫院本院',
    hosp_name: '台灣基督長老教會馬偕醫療財團法人新竹馬偕紀念醫院',
    hosp_class: '北區',
    hosp_city: '新竹市',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '30',
    location: [24.8000704, 120.9907507]
  },
  {
    hosp_desc: '1121010018台灣基督長老教會新樓醫療財團法人台南新樓醫院本院',
    hosp_name: '台灣基督長老教會新樓醫療財團法人台南新樓醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '10',
    location: [22.989311, 120.213287]
  },
  {
    hosp_desc: '1122010012戴德森醫療財團法人嘉義基督教醫院本院',
    hosp_name: '戴德森醫療財團法人嘉義基督教醫院',
    hosp_class: '南區',
    hosp_city: '嘉義市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '45',
    location: [23.4994174, 120.450168]
  },
  {
    hosp_desc:
      '1122010021天主教中華聖母修女會醫療財團法人天主教聖馬爾定醫院本院',
    hosp_name: '天主教中華聖母修女會醫療財團法人天主教聖馬爾定醫院',
    hosp_class: '南區',
    hosp_city: '嘉義市',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '14',
    location: [23.477034, 120.468146]
  },
  {
    hosp_desc:
      '1122010021天主教中華聖母修女會醫療財團法人天主教聖馬爾定醫院精神科醫院',
    hosp_name: '天主教中華聖母修女會醫療財團法人天主教聖馬爾定醫院',
    hosp_class: '南區',
    hosp_city: '嘉義市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.484121, 120.461639]
  },
  {
    hosp_desc: '1131090019行天宮醫療志業醫療財團法人恩主公醫院本院',
    hosp_name: '行天宮醫療志業醫療財團法人恩主公醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '33',
    location: [24.9383074, 121.362904]
  },
  {
    hosp_desc: '1131110516國泰醫療財團法人汐止國泰綜合醫院本院',
    hosp_name: '國泰醫療財團法人汐止國泰綜合醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '31',
    location: [25.0725373, 121.6611972]
  },
  {
    hosp_desc: '1132010024沙爾德聖保祿修女會醫療財團法人聖保祿醫院本院',
    hosp_name: '沙爾德聖保祿修女會醫療財團法人聖保祿醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '15',
    location: [24.9823171, 121.312003]
  },
  {
    hosp_desc: '1134020019醫療財團法人羅許基金會羅東博愛醫院本院',
    hosp_name: '醫療財團法人羅許基金會羅東博愛醫院',
    hosp_class: '台北區',
    hosp_city: '宜蘭縣',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '28',
    location: [24.6717649, 121.7729535]
  },
  {
    hosp_desc: '1134020028天主教靈醫會醫療財團法人羅東聖母醫院本院',
    hosp_name: '天主教靈醫會醫療財團法人羅東聖母醫院',
    hosp_class: '台北區',
    hosp_city: '宜蘭縣',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '15',
    location: [24.6718382, 121.7715737]
  },
  {
    hosp_desc: '1135050020為恭醫療財團法人為恭紀念醫院本院',
    hosp_name: '為恭醫療財團法人為恭紀念醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '15',
    location: [24.6869812, 120.907311]
  },
  {
    hosp_desc: '1135050020為恭醫療財團法人為恭紀念醫院東興院區',
    hosp_name: '為恭醫療財團法人為恭紀念醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.6689919, 120.9173461]
  },
  {
    hosp_desc: '1135050020為恭醫療財團法人為恭紀念醫院仁愛院區',
    hosp_name: '為恭醫療財團法人為恭紀念醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.6875172, 120.9083048]
  },
  {
    hosp_desc: '1136090519佛教慈濟醫療財團法人台中慈濟醫院本院',
    hosp_name: '佛教慈濟醫療財團法人台中慈濟醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '22',
    location: [24.1940115, 120.7205622]
  },
  {
    hosp_desc: '1136200015仁愛醫療財團法人大里仁愛醫院本院',
    hosp_name: '仁愛醫療財團法人大里仁愛醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '13',
    location: [24.1092272, 120.6805749]
  },
  {
    hosp_desc: '1137020511秀傳醫療財團法人彰濱秀傳紀念醫院本院',
    hosp_name: '秀傳醫療財團法人彰濱秀傳紀念醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '10',
    location: [24.077872, 120.408847]
  },
  {
    hosp_desc: '1138020015埔基醫療財團法人埔里基督教醫院本院',
    hosp_name: '埔基醫療財團法人埔里基督教醫院',
    hosp_class: '中區',
    hosp_city: '南投縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '15',
    location: [23.971399, 120.946553]
  },
  {
    hosp_desc: '1140010510長庚醫療財團法人嘉義長庚紀念醫院本院',
    hosp_name: '長庚醫療財團法人嘉義長庚紀念醫院',
    hosp_class: '南區',
    hosp_city: '嘉義縣',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '30',
    location: [23.4623035, 120.2859553]
  },
  {
    hosp_desc: '1140030012佛教慈濟醫療財團法人大林慈濟醫院本院',
    hosp_name: '佛教慈濟醫療財團法人大林慈濟醫院',
    hosp_class: '南區',
    hosp_city: '嘉義縣',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '15',
    location: [23.596561, 120.4571827]
  },
  {
    hosp_desc: '1141090512奇美醫療財團法人柳營奇美醫院本院',
    hosp_name: '奇美醫療財團法人柳營奇美醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '24',
    location: [23.2893229, 120.3247413]
  },
  {
    hosp_desc: '1143010012屏基醫療財團法人屏東基督教醫院本院',
    hosp_name: '屏基醫療財團法人屏東基督教醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '20',
    location: [22.6819849, 120.5034153]
  },
  {
    hosp_desc: '1143010012屏基醫療財團法人屏東基督教醫院瑞光院區',
    hosp_name: '屏基醫療財團法人屏東基督教醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6809704, 120.5053056]
  },
  {
    hosp_desc: '1145010038臺灣基督教門諾會醫療財團法人門諾醫院本院',
    hosp_name: '臺灣基督教門諾會醫療財團法人門諾醫院',
    hosp_class: '東區',
    hosp_city: '花蓮縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '8',
    location: [23.9885959, 121.626533]
  },
  {
    hosp_desc: '1146010014台灣基督長老教會馬偕醫療財團法人台東馬偕紀念醫院本院',
    hosp_name: '台灣基督長老教會馬偕醫療財團法人台東馬偕紀念醫院',
    hosp_class: '東區',
    hosp_city: '臺東縣',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '10',
    location: [22.7511439, 121.1408337]
  },
  {
    hosp_desc: '1231050017天主教耕莘醫療財團法人耕莘醫院本院',
    hosp_name: '天主教耕莘醫療財團法人耕莘醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '20',
    location: [24.976293, 121.5357523]
  },
  {
    hosp_desc: '1301170017臺北醫學大學附設醫院本院',
    hosp_name: '臺北醫學大學附設醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '25',
    location: [25.0270409, 121.5631091]
  },
  {
    hosp_desc: '1305370013臺南市立安南醫院-委託中國醫藥大學興建經營本院',
    hosp_name: '臺南市立安南醫院-委託中國醫藥大學興建經營',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '21',
    location: [23.0648363, 120.222761]
  },
  {
    hosp_desc: '1339060017中國醫藥大學北港附設醫院本院',
    hosp_name: '中國醫藥大學北港附設醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '11',
    location: [23.590144, 120.307229]
  },
  {
    hosp_desc: '1343030018輔英科技大學附設醫院本院',
    hosp_name: '輔英科技大學附設醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '15',
    location: [22.465312, 120.453052]
  },
  {
    hosp_desc: '1517011112澄清綜合醫院本院',
    hosp_name: '澄清綜合醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '10',
    location: [24.1426195, 120.681683]
  },
  {
    hosp_desc: '1517061032澄清綜合醫院中港分院本院',
    hosp_name: '澄清綜合醫院中港分院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '重度',
    hosp_ranking: '10',
    hosp_erbed: '20',
    location: [24.1828843, 120.6170212]
  },
  {
    hosp_desc: '1517061032澄清綜合醫院中港分院附設門診(臺中市敬德街8之1號)',
    hosp_name: '澄清綜合醫院中港分院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1984922, 120.6104502]
  },
  {
    hosp_desc: '1517061032澄清綜合醫院中港分院敬義樓',
    hosp_name: '澄清綜合醫院中港分院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1836396, 120.6170045]
  },
  {
    hosp_desc: '1532011154敏盛綜合醫院本院',
    hosp_name: '敏盛綜合醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '15',
    location: [25.0163919, 121.3063525]
  },
  {
    hosp_desc: '1532011154敏盛綜合醫院三民院區',
    hosp_name: '敏盛綜合醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9942874, 121.3054279]
  },
  {
    hosp_desc: '1532100049聯新國際醫院本院',
    hosp_name: '聯新國際醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '13',
    location: [24.9466922, 121.2053006]
  },
  {
    hosp_desc: '1543010109國仁醫院本院',
    hosp_name: '國仁醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '2',
    location: [22.6583156, 120.5145497]
  },
  {
    hosp_desc: '0102080017高雄市立民生醫院本院',
    hosp_name: '高雄市立民生醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '5',
    location: [22.628341, 120.322925]
  },
  {
    hosp_desc: '0122020517衛生福利部嘉義醫院本院',
    hosp_name: '衛生福利部嘉義醫院',
    hosp_class: '南區',
    hosp_city: '嘉義市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '3',
    location: [23.4810889, 120.4293213]
  },
  {
    hosp_desc: '0131060010衛生福利部樂生療養院療養院',
    hosp_name: '衛生福利部樂生療養院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '10',
    location: [25.0203597, 121.4079152]
  },
  {
    hosp_desc: '0132110519衛生福利部桃園醫院新屋分院本院',
    hosp_name: '衛生福利部桃園醫院新屋分院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '6',
    location: [24.9697419, 121.1069563]
  },
  {
    hosp_desc: '0140010028衛生福利部朴子醫院本院',
    hosp_name: '衛生福利部朴子醫院',
    hosp_class: '南區',
    hosp_city: '嘉義縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '4',
    location: [23.4644524, 120.2346391]
  },
  {
    hosp_desc: '0141010013衛生福利部新營醫院本院',
    hosp_name: '衛生福利部新營醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '6',
    location: [23.3089232, 120.3133117]
  },
  {
    hosp_desc: '0141060513衛生福利部臺南醫院新化分院本院',
    hosp_name: '衛生福利部臺南醫院新化分院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '一般',
    hosp_ranking: '3',
    hosp_erbed: '6',
    location: [23.063193, 120.335879]
  },
  {
    hosp_desc: '0141270019衛生福利部胸腔病院本院',
    hosp_name: '衛生福利部胸腔病院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.9744222, 120.2425159]
  },
  {
    hosp_desc: '0142030019衛生福利部旗山醫院本院',
    hosp_name: '衛生福利部旗山醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '6',
    location: [22.8807955, 120.483264]
  },
  {
    hosp_desc: '0143040019衛生福利部恆春旅遊醫院本院',
    hosp_name: '衛生福利部恆春旅遊醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '一般',
    hosp_ranking: '4',
    hosp_erbed: '7',
    location: [21.9992018, 120.7448456]
  },
  {
    hosp_desc: '0145010019衛生福利部花蓮醫院本院',
    hosp_name: '衛生福利部花蓮醫院',
    hosp_class: '東區',
    hosp_city: '花蓮縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '6',
    location: [23.9794291, 121.6111823]
  },
  {
    hosp_desc: '0145030020衛生福利部玉里醫院本院',
    hosp_name: '衛生福利部玉里醫院',
    hosp_class: '東區',
    hosp_city: '花蓮縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.3474832, 121.3210036]
  },
  {
    hosp_desc: '0145080011衛生福利部花蓮醫院豐濱原住民分院本院',
    hosp_name: '衛生福利部花蓮醫院豐濱原住民分院',
    hosp_class: '東區',
    hosp_city: '花蓮縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '3',
    location: [23.601916, 121.5211876]
  },
  {
    hosp_desc: '0146010013衛生福利部臺東醫院本院',
    hosp_name: '衛生福利部臺東醫院',
    hosp_class: '東區',
    hosp_city: '臺東縣',
    hosp_injury: '一般',
    hosp_ranking: '3',
    hosp_erbed: '3',
    location: [22.7572522, 121.1499505]
  },
  {
    hosp_desc: '0146020537衛生福利部臺東醫院成功分院本院',
    hosp_name: '衛生福利部臺東醫院成功分院',
    hosp_class: '東區',
    hosp_city: '臺東縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [23.0993473, 121.3778374]
  },
  {
    hosp_desc: '0211070012基隆市立醫院本院',
    hosp_name: '基隆市立醫院',
    hosp_class: '台北區',
    hosp_city: '基隆市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.1296063, 121.7619835]
  },
  {
    hosp_desc: '0401020013國立臺灣大學醫學院附設癌醫中心醫院本院',
    hosp_name: '國立臺灣大學醫學院附設癌醫中心醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0145031, 121.5441315]
  },
  {
    hosp_desc: '0401190010國立臺灣大學醫學院附設醫院北護分院本院',
    hosp_name: '國立臺灣大學醫學院附設醫院北護分院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0417202, 121.5035737]
  },
  {
    hosp_desc: '0431270012國立臺灣大學醫學院附設醫院金山分院本院',
    hosp_name: '國立臺灣大學醫學院附設醫院金山分院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '5',
    location: [25.2195534, 121.628399]
  },
  {
    hosp_desc: '0433030016國立臺灣大學醫學院附設醫院竹東分院本院',
    hosp_name: '國立臺灣大學醫學院附設醫院竹東分院',
    hosp_class: '北區',
    hosp_city: '新竹縣',
    hosp_injury: '一般',
    hosp_ranking: '4',
    hosp_erbed: '9',
    location: [25.2195534, 121.628399]
  },
  {
    hosp_desc: '0439010527國立成功大學醫學院附設醫院斗六分院本院',
    hosp_name: '國立成功大學醫學院附設醫院斗六分院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '一般',
    hosp_ranking: '4',
    hosp_erbed: '12',
    location: [23.703135, 120.5454919]
  },
  {
    hosp_desc: '0511040010三軍總醫院附設基隆民眾診療服務處本院',
    hosp_name: '三軍總醫院附設基隆民眾診療服務處',
    hosp_class: '台北區',
    hosp_city: '基隆市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '4',
    location: [25.1289081, 121.7394452]
  },
  {
    hosp_desc: '0512040014國軍新竹地區醫院附設民眾診療服務處本院',
    hosp_name: '國軍新竹地區醫院附設民眾診療服務處',
    hosp_class: '北區',
    hosp_city: '新竹市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '12',
    location: [24.8161603, 120.96212]
  },
  {
    hosp_desc: '0517050010國軍台中總醫院附設民眾診療服務處中清分院本院',
    hosp_name: '國軍台中總醫院附設民眾診療服務處中清分院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '4',
    location: [24.1621957, 120.6721757]
  },
  {
    hosp_desc: '0542020011國軍高雄總醫院岡山分院附設民眾診療服務處本院',
    hosp_name: '國軍高雄總醫院岡山分院附設民眾診療服務處',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '10',
    location: [22.7893873, 120.2855541]
  },
  {
    hosp_desc: '0543010019國軍高雄總醫院附設屏東民眾診療服務處本院',
    hosp_name: '國軍高雄總醫院附設屏東民眾診療服務處',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '3',
    location: [22.6336378, 120.4875275]
  },
  {
    hosp_desc: '0633030010臺北榮民總醫院新竹分院本院',
    hosp_name: '臺北榮民總醫院新竹分院',
    hosp_class: '北區',
    hosp_city: '新竹縣',
    hosp_injury: '一般',
    hosp_ranking: '5',
    hosp_erbed: '12',
    location: [24.7228808, 121.0993312]
  },
  {
    hosp_desc: '0634030014臺北榮民總醫院蘇澳分院本院',
    hosp_name: '臺北榮民總醫院蘇澳分院',
    hosp_class: '台北區',
    hosp_city: '宜蘭縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '5',
    location: [24.6134543, 121.8491838]
  },
  {
    hosp_desc: '0634070018臺北榮民總醫院員山分院本院',
    hosp_name: '臺北榮民總醫院員山分院',
    hosp_class: '台北區',
    hosp_city: '宜蘭縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '4',
    location: [24.7219914, 121.6872493]
  },
  {
    hosp_desc: '0638020014臺中榮民總醫院埔里分院本院',
    hosp_name: '臺中榮民總醫院埔里分院',
    hosp_class: '中區',
    hosp_city: '南投縣',
    hosp_injury: '一般',
    hosp_ranking: '4',
    hosp_erbed: '5',
    location: [23.9787547, 120.9943669]
  },
  {
    hosp_desc: '0640140012臺中榮民總醫院灣橋分院灣橋院區',
    hosp_name: '臺中榮民總醫院灣橋分院',
    hosp_class: '南區',
    hosp_city: '嘉義縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '2',
    location: [23.4876922, 120.5065154]
  },
  {
    hosp_desc: '0641310018高雄榮民總醫院臺南分院本院',
    hosp_name: '高雄榮民總醫院臺南分院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '8',
    location: [22.9972963, 120.2402196]
  },
  {
    hosp_desc: '0643130018高雄榮民總醫院屏東分院本院',
    hosp_name: '高雄榮民總醫院屏東分院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '8',
    location: [22.6744975, 120.5990661]
  },
  {
    hosp_desc: '0645020015臺北榮民總醫院鳳林分院本院',
    hosp_name: '臺北榮民總醫院鳳林分院',
    hosp_class: '東區',
    hosp_city: '花蓮縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '4',
    location: [23.7266005, 121.439958]
  },
  {
    hosp_desc: '0645030011臺北榮民總醫院玉里分院本院',
    hosp_name: '臺北榮民總醫院玉里分院',
    hosp_class: '東區',
    hosp_city: '花蓮縣',
    hosp_injury: '一般',
    hosp_ranking: '4',
    hosp_erbed: '4',
    location: [23.3392092, 121.3120416]
  },
  {
    hosp_desc: '0646010013臺北榮民總醫院臺東分院本院',
    hosp_name: '臺北榮民總醫院臺東分院',
    hosp_class: '東區',
    hosp_city: '臺東縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '2',
    location: [22.772343, 121.1325704]
  },
  {
    hosp_desc: '0717070516法務部矯正署臺中監獄附設培德醫院本院',
    hosp_name: '法務部矯正署臺中監獄附設培德醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [24.1423177, 120.5984766]
  },
  {
    hosp_desc: '0901020013中山醫療社團法人中山醫院本院',
    hosp_name: '中山醫療社團法人中山醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '6',
    location: [25.0365128, 121.5499646]
  },
  {
    hosp_desc: '0901180023郵政醫院（委託中英醫療社團法人經營）本院',
    hosp_name: '郵政醫院（委託中英醫療社團法人經營）',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [25.0287244, 121.5187299]
  },
  {
    hosp_desc: '0901190010西園醫療社團法人西園醫院本院',
    hosp_name: '西園醫療社團法人西園醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '9',
    location: [25.0277351, 121.4944414]
  },
  {
    hosp_desc: '0903150014林新醫療社團法人烏日林新醫院本院',
    hosp_name: '林新醫療社團法人烏日林新醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '5',
    location: [24.1106491, 120.5978941]
  },
  {
    hosp_desc: '0905320014仁愛醫療社團法人仁愛醫院本院',
    hosp_name: '仁愛醫療社團法人仁愛醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.9913749, 120.2113844]
  },
  {
    hosp_desc: '0907030013廣聖醫療社團法人廣聖醫院本院',
    hosp_name: '廣聖醫療社團法人廣聖醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.8849805, 120.4854746]
  },
  {
    hosp_desc: '0907320012愛仁醫療社團法人愛仁醫院本院',
    hosp_name: '愛仁醫療社團法人愛仁醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6417975, 120.3138514]
  },
  {
    hosp_desc: '0907350010乃榮醫療社團法人乃榮醫院本院',
    hosp_name: '乃榮醫療社團法人乃榮醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [22.6187459, 120.318798]
  },
  {
    hosp_desc: '0922020013仁德醫療社團法人陳仁德醫院本院',
    hosp_name: '仁德醫療社團法人陳仁德醫院',
    hosp_class: '南區',
    hosp_city: '嘉義市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [23.4800516, 120.443302]
  },
  {
    hosp_desc: '0922020022慶昇醫療社團法人慶昇醫院本院',
    hosp_name: '慶昇醫療社團法人慶昇醫院',
    hosp_class: '南區',
    hosp_city: '嘉義市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [23.480082, 120.4439489]
  },
  {
    hosp_desc: '0931010016中英醫療社團法人中英醫院本院',
    hosp_name: '中英醫療社團法人中英醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0199064, 121.4662881]
  },
  {
    hosp_desc: '0931010025中英醫療社團法人板英醫院本院',
    hosp_name: '中英醫療社團法人板英醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0187829, 121.4652441]
  },
  {
    hosp_desc: '0931060016新仁醫療社團法人新仁醫院本院',
    hosp_name: '新仁醫療社團法人新仁醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [25.0344689, 121.4454927]
  },
  {
    hosp_desc: '0931090014永聖醫療社團法人文化醫院本院',
    hosp_name: '永聖醫療社團法人文化醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9322201, 121.3760214]
  },
  {
    hosp_desc: '0932020016宏其醫療社團法人宏其婦幼醫院本院',
    hosp_name: '宏其醫療社團法人宏其婦幼醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9611392, 121.2270322]
  },
  {
    hosp_desc: '0932020034新國民醫療社團法人新國民醫院本院',
    hosp_name: '新國民醫療社團法人新國民醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9541537, 121.223528]
  },
  {
    hosp_desc: '0935010012梓榮醫療社團法人弘大醫院本院',
    hosp_name: '梓榮醫療社團法人弘大醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.5515087, 120.8209267]
  },
  {
    hosp_desc: '0935020027李綜合醫療社團法人苑裡李綜合醫院本院',
    hosp_name: '李綜合醫療社團法人苑裡李綜合醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '6',
    location: [24.4419049, 120.652647]
  },
  {
    hosp_desc: '0937030012道周醫療社團法人道周醫院本院',
    hosp_name: '道周醫療社團法人道周醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '2',
    location: [24.1106413, 120.4919836]
  },
  {
    hosp_desc: '0937050014員榮醫療社團法人員榮醫院本院',
    hosp_name: '員榮醫療社團法人員榮醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '9',
    location: [23.9536637, 120.5747744]
  },
  {
    hosp_desc: '0937050024惠來醫療社團法人宏仁醫院本院',
    hosp_name: '惠來醫療社團法人宏仁醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [23.9549665, 120.5702996]
  },
  {
    hosp_desc: '0937080012洪宗鄰醫療社團法人洪宗鄰醫院本院',
    hosp_name: '洪宗鄰醫療社團法人洪宗鄰醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.8989217, 120.3678286]
  },
  {
    hosp_desc: '0938030016佑民醫療社團法人佑民醫院本院',
    hosp_name: '佑民醫療社團法人佑民醫院',
    hosp_class: '中區',
    hosp_city: '南投縣',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '14',
    location: [23.9606022, 120.6817657]
  },
  {
    hosp_desc: '0941010019新興醫療社團法人新興醫院本院',
    hosp_name: '新興醫療社團法人新興醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.3037389, 120.3168546]
  },
  {
    hosp_desc: '0941310014永達醫療社團法人永達醫院本院',
    hosp_name: '永達醫療社團法人永達醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.022175, 120.2614491]
  },
  {
    hosp_desc: '0942020019高雄市立岡山醫院（委託秀傳醫療社團法人經營）本院',
    hosp_name: '高雄市立岡山醫院（委託秀傳醫療社團法人經營）',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '10',
    location: [22.7965499, 120.2946285]
  },
  {
    hosp_desc: '0943010026安和醫療社團法人安和醫院本院',
    hosp_name: '安和醫療社團法人安和醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [22.6834768, 120.4836687]
  },
  {
    hosp_desc: '0943010035優生醫療社團法人優生醫院本院',
    hosp_name: '優生醫療社團法人優生醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6860494, 120.5038553]
  },
  {
    hosp_desc: '0943020013安泰醫療社團法人潮州安泰醫院本院',
    hosp_name: '安泰醫療社團法人潮州安泰醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '一般',
    hosp_ranking: '4',
    hosp_erbed: '3',
    location: [22.553859, 120.547211]
  },
  {
    hosp_desc: '0943040015南門醫療社團法人南門醫院本院',
    hosp_name: '南門醫療社團法人南門醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '一般',
    hosp_ranking: '4',
    hosp_erbed: '2',
    location: [22.0009266, 120.7453672]
  },
  {
    hosp_desc: '0943160012枋寮醫療社團法人枋寮醫院本院',
    hosp_name: '枋寮醫療社團法人枋寮醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '11',
    location: [22.3639316, 120.5963465]
  },
  {
    hosp_desc: '1101020027中心診所醫療財團法人中心綜合醫院本院',
    hosp_name: '中心診所醫療財團法人中心綜合醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '8',
    location: [25.0418484, 121.547396]
  },
  {
    hosp_desc: '1101020036宏恩醫療財團法人宏恩綜合醫院本院',
    hosp_name: '宏恩醫療財團法人宏恩綜合醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [25.0381916, 121.5466881]
  },
  {
    hosp_desc: '1101110026康寧醫療財團法人康寧醫院本院',
    hosp_name: '康寧醫療財團法人康寧醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '2',
    location: [25.0758907, 121.6089476]
  },
  {
    hosp_desc: '1105050012奇美醫療財團法人佳里奇美醫院本院',
    hosp_name: '奇美醫療財團法人佳里奇美醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '20',
    location: [23.1816689, 120.1837918]
  },
  {
    hosp_desc: '1107120017義大醫療財團法人義大癌治療醫院本院',
    hosp_name: '義大醫療財團法人義大癌治療醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '10',
    location: [22.7641541, 120.3616921]
  },
  {
    hosp_desc: '1107320017義大醫療財團法人義大大昌醫院本院',
    hosp_name: '義大醫療財團法人義大大昌醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6577314, 120.3198932]
  },
  {
    hosp_desc: '1107350015天主教聖功醫療財團法人聖功醫院本院',
    hosp_name: '天主教聖功醫療財團法人聖功醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '5',
    location: [22.6333516, 120.3239848]
  },
  {
    hosp_desc: '1112010528國泰醫療財團法人新竹國泰綜合醫院本院',
    hosp_name: '國泰醫療財團法人新竹國泰綜合醫院',
    hosp_class: '北區',
    hosp_city: '新竹市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '37',
    location: [24.7982349, 120.9653132]
  },
  {
    hosp_desc: '1117010019仁愛醫療財團法人台中仁愛醫院本院',
    hosp_name: '仁愛醫療財團法人台中仁愛醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1433996, 120.6778803]
  },
  {
    hosp_desc: '1132071036長庚醫療財團法人桃園長庚紀念醫院本院',
    hosp_name: '長庚醫療財團法人桃園長庚紀念醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [25.030638, 121.366919]
  },
  {
    hosp_desc: '1133060019天主教仁慈醫療財團法人仁慈醫院本院',
    hosp_name: '天主教仁慈醫療財團法人仁慈醫院',
    hosp_class: '北區',
    hosp_city: '新竹縣',
    hosp_injury: '一般',
    hosp_ranking: '3',
    hosp_erbed: '10',
    location: [24.9004859, 121.0455461]
  },
  {
    hosp_desc: '1134010022宜蘭仁愛醫療財團法人宜蘭仁愛醫院本院',
    hosp_name: '宜蘭仁愛醫療財團法人宜蘭仁愛醫院',
    hosp_class: '台北區',
    hosp_city: '宜蘭縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '4',
    location: [24.7487742, 121.7545312]
  },
  {
    hosp_desc: '1137010051彰化基督教醫療財團法人漢銘基督教醫院本院',
    hosp_name: '彰化基督教醫療財團法人漢銘基督教醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.0612669, 120.5356455]
  },
  {
    hosp_desc: '1137020520彰化基督教醫療財團法人鹿港基督教醫院本院',
    hosp_name: '彰化基督教醫療財團法人鹿港基督教醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '8',
    location: [24.0604617, 120.4382386]
  },
  {
    hosp_desc: '1137050019彰化基督教醫療財團法人員林基督教醫院本院',
    hosp_name: '彰化基督教醫療財團法人員林基督教醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '33',
    location: [23.962824, 120.5672136]
  },
  {
    hosp_desc: '1137080017彰化基督教醫療財團法人二林基督教醫院本院',
    hosp_name: '彰化基督教醫療財團法人二林基督教醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '13',
    location: [23.8935714, 120.3633791]
  },
  {
    hosp_desc: '1139010013佛教慈濟醫療財團法人斗六慈濟醫院本院',
    hosp_name: '佛教慈濟醫療財團法人斗六慈濟醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.7022825, 120.5299519]
  },
  {
    hosp_desc: '1139020019天主教中華道明修女會醫療財團法人天主教福安醫院本院',
    hosp_name: '天主教中華道明修女會醫療財團法人天主教福安醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.6788321, 120.482127]
  },
  {
    hosp_desc: '1139030015天主教若瑟醫療財團法人若瑟醫院本院',
    hosp_name: '天主教若瑟醫療財團法人若瑟醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '10',
    location: [23.7080161, 120.4373426]
  },
  {
    hosp_desc: '1139040011彰化基督教醫療財團法人雲林基督教醫院本院',
    hosp_name: '彰化基督教醫療財團法人雲林基督教醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '12',
    location: [23.7815488, 120.4412736]
  },
  {
    hosp_desc: '1139130010長庚醫療財團法人雲林長庚紀念醫院本院',
    hosp_name: '長庚醫療財團法人雲林長庚紀念醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '一般',
    hosp_ranking: '4',
    hosp_erbed: '9',
    location: [23.7956448, 120.2187259]
  },
  {
    hosp_desc: '1142010518高雄市立鳳山醫院（委託長庚醫療財團法人經營）本院',
    hosp_name: '高雄市立鳳山醫院（委託長庚醫療財團法人經營）',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '6',
    location: [22.62883, 120.362788]
  },
  {
    hosp_desc: '1143040010恆基醫療財團法人恆春基督教醫院本院',
    hosp_name: '恆基醫療財團法人恆春基督教醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '3',
    location: [22.0023386, 120.7410562]
  },
  {
    hosp_desc: '1145030012佛教慈濟醫療財團法人玉里慈濟醫院本院',
    hosp_name: '佛教慈濟醫療財團法人玉里慈濟醫院',
    hosp_class: '東區',
    hosp_city: '花蓮縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '5',
    location: [23.334904, 121.318599]
  },
  {
    hosp_desc: '1146010032東基醫療財團法人台東基督教醫院本院',
    hosp_name: '東基醫療財團法人台東基督教醫院',
    hosp_class: '東區',
    hosp_city: '臺東縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '5',
    location: [22.7638706, 121.1462787]
  },
  {
    hosp_desc: '1146010041天主教花蓮教區醫療財團法人台東聖母醫院本院',
    hosp_name: '天主教花蓮教區醫療財團法人台東聖母醫院',
    hosp_class: '東區',
    hosp_city: '臺東縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [22.7577179, 121.1462936]
  },
  {
    hosp_desc: '1146030516佛教慈濟醫療財團法人關山慈濟醫院本院',
    hosp_name: '佛教慈濟醫療財團法人關山慈濟醫院',
    hosp_class: '東區',
    hosp_city: '臺東縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '4',
    location: [23.057294, 121.1669677]
  },
  {
    hosp_desc: '1202080029信義醫療財團法人高雄基督教醫院本院',
    hosp_name: '信義醫療財團法人高雄基督教醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6139795, 120.296308]
  },
  {
    hosp_desc: '1231030015天主教耕莘醫療財團法人永和耕莘醫院本院',
    hosp_name: '天主教耕莘醫療財團法人永和耕莘醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '10',
    location: [25.0118746, 121.5177337]
  },
  {
    hosp_desc: '1301110511中國醫藥大學附設醫院臺北分院本院',
    hosp_name: '中國醫藥大學附設醫院臺北分院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0821265, 121.5907054]
  },
  {
    hosp_desc: '1303180011亞洲大學附屬醫院本院',
    hosp_name: '亞洲大學附屬醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '10',
    location: [24.0536223, 120.6860365]
  },
  {
    hosp_desc:
      '1307370011高雄市立旗津醫院(委託財團法人私立高雄醫學大學經營)本院',
    hosp_name: '高雄市立旗津醫院(委託財團法人私立高雄醫學大學經營)',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '3',
    location: [22.5904375, 120.2851625]
  },
  {
    hosp_desc: '1317020519中國醫藥大學附設醫院台中東區分院本院',
    hosp_name: '中國醫藥大學附設醫院台中東區分院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1425786, 120.694811]
  },
  {
    hosp_desc: '1317040039中山醫學大學附設醫院中興分院本院',
    hosp_name: '中山醫學大學附設醫院中興分院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [24.1180933, 120.6585828]
  },
  {
    hosp_desc: '1331160010輔仁大學學校財團法人輔仁大學附設醫院本院',
    hosp_name: '輔仁大學學校財團法人輔仁大學附設醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '23',
    location: [25.0395655, 121.4312129]
  },
  {
    hosp_desc: '1333050017中國醫藥大學新竹附設醫院本院',
    hosp_name: '中國醫藥大學新竹附設醫院',
    hosp_class: '北區',
    hosp_city: '新竹縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '12',
    location: [24.8205437, 121.0070159]
  },
  {
    hosp_desc: '1336010015中國醫藥大學附設醫院豐原分院本院',
    hosp_name: '中國醫藥大學附設醫院豐原分院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.2516467, 120.7183696]
  },
  {
    hosp_desc: '1338030015中國醫藥大學附設醫院草屯分院本院',
    hosp_name: '中國醫藥大學附設醫院草屯分院',
    hosp_class: '中區',
    hosp_city: '南投縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.9847733, 120.6859284]
  },
  {
    hosp_desc: '1401190011財團法人台灣省私立台北仁濟院附設仁濟醫院本院',
    hosp_name: '財團法人台灣省私立台北仁濟院附設仁濟醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [25.0366809, 121.4980941]
  },
  {
    hosp_desc: '1411030013醫療財團法人臺灣區煤礦業基金會臺灣礦工醫院本院',
    hosp_name: '醫療財團法人臺灣區煤礦業基金會臺灣礦工醫院',
    hosp_class: '台北區',
    hosp_city: '基隆市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '6',
    location: [25.1078781, 121.733227]
  },
  {
    hosp_desc: '1412040022財團法人台灣省私立桃園仁愛之家附設新竹新生醫院本院',
    hosp_name: '財團法人台灣省私立桃園仁愛之家附設新竹新生醫院',
    hosp_class: '北區',
    hosp_city: '新竹市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.8016053, 120.9634921]
  },
  {
    hosp_desc: '1436020013東勢區農會附設農民醫院本院',
    hosp_name: '東勢區農會附設農民醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '2',
    location: [24.2534386, 120.8291464]
  },
  {
    hosp_desc: '1501010010博仁綜合醫院本院',
    hosp_name: '博仁綜合醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '4',
    location: [25.0499097, 121.5576835]
  },
  {
    hosp_desc: '1501021219秀傳醫院本院',
    hosp_name: '秀傳醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0431041, 121.5568209]
  },
  {
    hosp_desc: '1501100037協和婦女醫院本院',
    hosp_name: '協和婦女醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0502613, 121.5339092]
  },
  {
    hosp_desc: '1501201020景美醫院本院',
    hosp_name: '景美醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '3',
    location: [24.991198, 121.5398813]
  },
  {
    hosp_desc: '1502020065正大醫院本院',
    hosp_name: '正大醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6638752, 120.2797023]
  },
  {
    hosp_desc: '1502031095馨蕙馨醫院本院',
    hosp_name: '馨蕙馨醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6609865, 120.3038197]
  },
  {
    hosp_desc: '1502031102柏仁醫院本院',
    hosp_name: '柏仁醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6642634, 120.3036424]
  },
  {
    hosp_desc: '1502040021健仁醫院本院',
    hosp_name: '健仁醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '4',
    location: [22.7237039, 120.3291328]
  },
  {
    hosp_desc: '1502040076顏威裕醫院本院',
    hosp_name: '顏威裕醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.7111144, 120.2956424]
  },
  {
    hosp_desc: '1502041108長春醫院本院',
    hosp_name: '長春醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.714334, 120.2891549]
  },
  {
    hosp_desc: '1502041117右昌聯合醫院本院',
    hosp_name: '右昌聯合醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.713498, 120.2909126]
  },
  {
    hosp_desc: '1502050045德謙醫院本院',
    hosp_name: '德謙醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.64066, 120.3131826]
  },
  {
    hosp_desc: '1502050170祐生醫院本院',
    hosp_name: '祐生醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6379608, 120.2961286]
  },
  {
    hosp_desc: '1502050241民族醫院本院',
    hosp_name: '民族醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6684156, 120.3172998]
  },
  {
    hosp_desc: '1502050296文雄醫院本院',
    hosp_name: '文雄醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6480612, 120.2995451]
  },
  {
    hosp_desc: '1502051337謝外科醫院本院',
    hosp_name: '謝外科醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6368065, 120.3031838]
  },
  {
    hosp_desc: '1502051426四季台安醫院本院',
    hosp_name: '四季台安醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6594191, 120.3137351]
  },
  {
    hosp_desc: '1502060014蕭志文醫院本院',
    hosp_name: '蕭志文醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6338044, 120.3142288]
  },
  {
    hosp_desc: '1502060112原祿骨科醫院本院',
    hosp_name: '原祿骨科醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6313708, 120.3088438]
  },
  {
    hosp_desc: '1502060149惠仁醫院本院',
    hosp_name: '惠仁醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6299088, 120.3015488]
  },
  {
    hosp_desc: '1502061208新華醫院本院',
    hosp_name: '新華醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.633414, 120.297147]
  },
  {
    hosp_desc: '1502070029重仁骨科醫院本院',
    hosp_name: '重仁骨科醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [22.6343543, 120.2937679]
  },
  {
    hosp_desc: '1502070118健新醫院本院',
    hosp_name: '健新醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6313368, 120.2908694]
  },
  {
    hosp_desc: '1502081175邱外科醫院本院',
    hosp_name: '邱外科醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '2',
    location: [22.6147008, 120.297335]
  },
  {
    hosp_desc: '1502090209吳昆哲婦產小兒科醫院本院',
    hosp_name: '吳昆哲婦產小兒科醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6120447, 120.3143343]
  },
  {
    hosp_desc: '1502091297佳欣婦幼醫院本院',
    hosp_name: '佳欣婦幼醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.615646, 120.3102381]
  },
  {
    hosp_desc: '1502110064安泰醫院本院',
    hosp_name: '安泰醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.5652043, 120.3613014]
  },
  {
    hosp_desc: '1502111089戴銘浚婦兒醫院本院',
    hosp_name: '戴銘浚婦兒醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.569592, 120.3511481]
  },
  {
    hosp_desc: '1503010018惠盛醫院本院',
    hosp_name: '惠盛醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.2518425, 120.7161566]
  },
  {
    hosp_desc: '1503010027杏豐醫院本院',
    hosp_name: '杏豐醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [24.253662, 120.720143]
  },
  {
    hosp_desc: '1503010036漢忠醫院本院',
    hosp_name: '漢忠醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [24.2522299, 120.7178434]
  },
  {
    hosp_desc: '1503030010順安醫院本院',
    hosp_name: '順安醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [24.3437551, 120.6254951]
  },
  {
    hosp_desc: '1503190020長安醫院本院',
    hosp_name: '長安醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '8',
    location: [24.1312122, 120.7196786]
  },
  {
    hosp_desc: '1503190039新太平澄清醫院本院',
    hosp_name: '新太平澄清醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [24.1273392, 120.7155459]
  },
  {
    hosp_desc: '1503200012霧峰澄清醫院本院',
    hosp_name: '霧峰澄清醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '6',
    location: [24.0854178, 120.6996074]
  },
  {
    hosp_desc: '1503260018臺安醫院雙十分院本院',
    hosp_name: '臺安醫院雙十分院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '6',
    location: [24.15271, 120.6886184]
  },
  {
    hosp_desc: '1503290016澄清復健醫院本院',
    hosp_name: '澄清復健醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1616562, 120.7215445]
  },
  {
    hosp_desc: '1503290025茂盛醫院本院',
    hosp_name: '茂盛醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.166639, 120.6940879]
  },
  {
    hosp_desc: '1505310011璟馨婦幼醫院本院',
    hosp_name: '璟馨婦幼醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.0219048, 120.2368743]
  },
  {
    hosp_desc: '1505340019大安婦幼醫院本院',
    hosp_name: '大安婦幼醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.9927341, 120.192872]
  },
  {
    hosp_desc: '1505350015陳澤彥婦產科醫院本院',
    hosp_name: '陳澤彥婦產科醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.0197088, 120.1984428]
  },
  {
    hosp_desc: '1507010014新高鳳醫院本院',
    hosp_name: '新高鳳醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6262003, 120.3587368]
  },
  {
    hosp_desc: '1507290012生安婦產小兒科醫院本院',
    hosp_name: '生安婦產小兒科醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6624537, 120.2882223]
  },
  {
    hosp_desc: '1507300022博愛蕙馨醫院本院',
    hosp_name: '博愛蕙馨醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '5',
    location: [22.6568875, 120.3035025]
  },
  {
    hosp_desc: '1507310019金安心醫院本院',
    hosp_name: '金安心醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.7159073, 120.2991948]
  },
  {
    hosp_desc: '1507320015新高醫院本院',
    hosp_name: '新高醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6499518, 120.3181974]
  },
  {
    hosp_desc: '1507330011七賢脊椎外科醫院本院',
    hosp_name: '七賢脊椎外科醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.634478, 120.3061616]
  },
  {
    hosp_desc: '1507340017中正脊椎骨科醫院本院',
    hosp_name: '中正脊椎骨科醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6304509, 120.2982768]
  },
  {
    hosp_desc: '1507340026上琳醫院本院',
    hosp_name: '上琳醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6312068, 120.2928641]
  },
  {
    hosp_desc: '1507340044活力得中山脊椎外科醫院本院',
    hosp_name: '活力得中山脊椎外科醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6200096, 120.3017257]
  },
  {
    hosp_desc: '1507360019瑞祥醫院本院',
    hosp_name: '瑞祥醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6028325, 120.3249444]
  },
  {
    hosp_desc: '1507360028新正薪醫院本院',
    hosp_name: '新正薪醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.60791, 120.317754]
  },
  {
    hosp_desc: '1511010068新昆明醫院本院',
    hosp_name: '新昆明醫院',
    hosp_class: '台北區',
    hosp_city: '基隆市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.1331296, 121.7448737]
  },
  {
    hosp_desc: '1512011185南門綜合醫院本院',
    hosp_name: '南門綜合醫院',
    hosp_class: '北區',
    hosp_city: '新竹市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '6',
    location: [24.8021381, 120.969653]
  },
  {
    hosp_desc: '1512040051新中興醫院本院',
    hosp_name: '新中興醫院',
    hosp_class: '北區',
    hosp_city: '新竹市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.8018917, 120.9628227]
  },
  {
    hosp_desc: '1517011103第一醫院本院',
    hosp_name: '第一醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.142505, 120.6764239]
  },
  {
    hosp_desc: '1517020040台新醫院本院',
    hosp_name: '台新醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [24.130363, 120.705561]
  },
  {
    hosp_desc: '1517021074臺安醫院本院',
    hosp_name: '臺安醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [24.149049, 120.6942496]
  },
  {
    hosp_desc: '1517030055林森醫院本院',
    hosp_name: '林森醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1358608, 120.6729764]
  },
  {
    hosp_desc: '1517040015宏恩醫院本院',
    hosp_name: '宏恩醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1202363, 120.6606359]
  },
  {
    hosp_desc: '1517050084新亞東婦產科醫院本院',
    hosp_name: '新亞東婦產科醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '3',
    location: [24.163785, 120.6741429]
  },
  {
    hosp_desc: '1517051107勝美醫院本院',
    hosp_name: '勝美醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1533073, 120.6829956]
  },
  {
    hosp_desc: '1517070031友仁醫院本院',
    hosp_name: '友仁醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [24.1398638, 120.6495284]
  },
  {
    hosp_desc: '1517080019聯安醫院本院',
    hosp_name: '聯安醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '4',
    location: [24.1709445, 120.6993312]
  },
  {
    hosp_desc: '1517080091全民醫院本院',
    hosp_name: '全民醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [24.1815926, 120.6664871]
  },
  {
    hosp_desc: '1517081141博愛外科醫院本院',
    hosp_name: '博愛外科醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.173492, 120.667943]
  },
  {
    hosp_desc: '1521030081洪外科醫院本院',
    hosp_name: '洪外科醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.995192, 120.197639]
  },
  {
    hosp_desc: '1521031104郭綜合醫院本院',
    hosp_name: '郭綜合醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '中度',
    hosp_ranking: '7',
    hosp_erbed: '8',
    location: [22.9947575, 120.1986977]
  },
  {
    hosp_desc: '1521040050志誠醫院本院',
    hosp_name: '志誠醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.0035883, 120.2100355]
  },
  {
    hosp_desc: '1521041137開元寺慈愛醫院本院',
    hosp_name: '開元寺慈愛醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.0107304, 120.2229174]
  },
  {
    hosp_desc: '1521050010永川醫院本院',
    hosp_name: '永川醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.9980965, 120.2062791]
  },
  {
    hosp_desc: '1521051160永和醫院本院',
    hosp_name: '永和醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.9902863, 120.1988861]
  },
  {
    hosp_desc: '1521051179仁村醫院本院',
    hosp_name: '仁村醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.987966, 120.1972997]
  },
  {
    hosp_desc: '1522011115陽明醫院本院',
    hosp_name: '陽明醫院',
    hosp_class: '南區',
    hosp_city: '嘉義市',
    hosp_injury: '一般',
    hosp_ranking: '4',
    hosp_erbed: '4',
    location: [23.4804759, 120.4535129]
  },
  {
    hosp_desc: '1522021175盧亞人醫院本院',
    hosp_name: '盧亞人醫院',
    hosp_class: '南區',
    hosp_city: '嘉義市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [23.4821683, 120.4475748]
  },
  {
    hosp_desc: '1522021264安心醫院本院',
    hosp_name: '安心醫院',
    hosp_class: '南區',
    hosp_city: '嘉義市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '4',
    location: [23.4535328, 120.4336569]
  },
  {
    hosp_desc: '1531010082板新醫院本院',
    hosp_name: '板新醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0168642, 121.4564036]
  },
  {
    hosp_desc: '1531010108蕭中正醫院本院',
    hosp_name: '蕭中正醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [25.0068854, 121.4564325]
  },
  {
    hosp_desc: '1531010279板橋中興醫院本院',
    hosp_name: '板橋中興醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '6',
    location: [25.0019608, 121.460159]
  },
  {
    hosp_desc: '1531011310板橋國泰醫院本院',
    hosp_name: '板橋國泰醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0019307, 121.4599294]
  },
  {
    hosp_desc: '1531020122宏仁醫院本院',
    hosp_name: '宏仁醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '3',
    location: [25.0689165, 121.4986338]
  },
  {
    hosp_desc: '1531021165三重中興醫院本院',
    hosp_name: '三重中興醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0465578, 121.4696738]
  },
  {
    hosp_desc: '1531021174祐民醫院本院',
    hosp_name: '祐民醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0628005, 121.4998815]
  },
  {
    hosp_desc: '1531031278永和復康醫院本院',
    hosp_name: '永和復康醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9997024, 121.5054981]
  },
  {
    hosp_desc: '1531040259中祥醫院本院',
    hosp_name: '中祥醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.003255, 121.5020897]
  },
  {
    hosp_desc: '1531041363蕙生醫院本院',
    hosp_name: '蕙生醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [25.005464, 121.480915]
  },
  {
    hosp_desc: '1531041390怡和醫院本院',
    hosp_name: '怡和醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0018899, 121.4985247]
  },
  {
    hosp_desc: '1531041407祥顥醫院本院',
    hosp_name: '祥顥醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9933355, 121.5082015]
  },
  {
    hosp_desc: '1531050077同仁醫院本院',
    hosp_name: '同仁醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.982687, 121.537736]
  },
  {
    hosp_desc: '1531051163豐榮醫院本院',
    hosp_name: '豐榮醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9632498, 121.5116681]
  },
  {
    hosp_desc: '1531051172新北仁康醫院本院',
    hosp_name: '新北仁康醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9555591, 121.4997383]
  },
  {
    hosp_desc: '1531060180新泰綜合醫院本院',
    hosp_name: '新泰綜合醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '6',
    location: [25.0214551, 121.4343264]
  },
  {
    hosp_desc: '1531061230益民醫院本院',
    hosp_name: '益民醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0401053, 121.4551363]
  },
  {
    hosp_desc: '1531071030仁愛醫院本院',
    hosp_name: '仁愛醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '8',
    location: [24.9871737, 121.4200433]
  },
  {
    hosp_desc: '1531091130清福醫院本院',
    hosp_name: '清福醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9300099, 121.379828]
  },
  {
    hosp_desc: '1531100027公祥醫院本院',
    hosp_name: '公祥醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.1699716, 121.4442784]
  },
  {
    hosp_desc: '1531120038瑞芳礦工醫院本院',
    hosp_name: '瑞芳礦工醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '3',
    location: [25.1086014, 121.8021627]
  },
  {
    hosp_desc: '1531130052廣川醫院本院',
    hosp_name: '廣川醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [24.9823815, 121.4549682]
  },
  {
    hosp_desc: '1531130105仁安醫院本院',
    hosp_name: '仁安醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.989568, 121.447319]
  },
  {
    hosp_desc: '1531131139元復醫院本院',
    hosp_name: '元復醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9728932, 121.4403173]
  },
  {
    hosp_desc: '1531131157恩樺醫院本院',
    hosp_name: '恩樺醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9912041, 121.4487575]
  },
  {
    hosp_desc: '1532010013振生醫院本院',
    hosp_name: '振生醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9997325, 121.3095098]
  },
  {
    hosp_desc: '1532010120聯新國際醫院桃新分院本院',
    hosp_name: '聯新國際醫院桃新分院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9891986, 121.3093204]
  },
  {
    hosp_desc: '1532011163德仁醫院本院',
    hosp_name: '德仁醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.984073, 121.3186219]
  },
  {
    hosp_desc: '1532020215祐民醫院本院',
    hosp_name: '祐民醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [24.9574699, 121.2037758]
  },
  {
    hosp_desc: '1532021285承安醫院本院',
    hosp_name: '承安醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9532979, 121.2219632]
  },
  {
    hosp_desc: '1532021338中壢長榮醫院本院',
    hosp_name: '中壢長榮醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '3',
    location: [24.9644779, 121.2578335]
  },
  {
    hosp_desc: '1532021365華揚醫院本院',
    hosp_name: '華揚醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9531213, 121.2299504]
  },
  {
    hosp_desc: '1532021374長慎醫院本院',
    hosp_name: '長慎醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.94735, 121.2462283]
  },
  {
    hosp_desc: '1532021383懷寧醫院本院',
    hosp_name: '懷寧醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '4',
    location: [24.9614439, 121.2054961]
  },
  {
    hosp_desc: '1532021392中美醫院本院',
    hosp_name: '中美醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9586345, 121.2251161]
  },
  {
    hosp_desc: '1532040039天成醫院天成院區',
    hosp_name: '天成醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '10',
    location: [24.9089106, 121.1572076]
  },
  {
    hosp_desc: '1532040066怡仁綜合醫院本院',
    hosp_name: '怡仁綜合醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '10',
    location: [24.9170236, 121.1555767]
  },
  {
    hosp_desc: '1532061065大園敏盛醫院本院',
    hosp_name: '大園敏盛醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0623117, 121.1959554]
  },
  {
    hosp_desc: '1532070019大明醫院本院',
    hosp_name: '大明醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9952431, 121.3376218]
  },
  {
    hosp_desc: '1532091081龍潭敏盛醫院本院',
    hosp_name: '龍潭敏盛醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.8751561, 121.2137771]
  },
  {
    hosp_desc: '1532100012新永和醫院本院',
    hosp_name: '新永和醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9514918, 121.2195401]
  },
  {
    hosp_desc: '1532101091陽明醫院本院',
    hosp_name: '陽明醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '6',
    location: [24.950095, 121.2124714]
  },
  {
    hosp_desc: '1532101108宋俊宏婦幼醫院本院',
    hosp_name: '宋俊宏婦幼醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9561253, 121.2100441]
  },
  {
    hosp_desc: '1532101117秉坤婦幼醫院本院',
    hosp_name: '秉坤婦幼醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '9',
    location: [24.9481938, 121.2114369]
  },
  {
    hosp_desc: '1533030028林醫院本院',
    hosp_name: '林醫院',
    hosp_class: '北區',
    hosp_city: '新竹縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.7368986, 121.0912516]
  },
  {
    hosp_desc: '1533030046竹信醫院本院',
    hosp_name: '竹信醫院',
    hosp_class: '北區',
    hosp_city: '新竹縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.734258, 121.090935]
  },
  {
    hosp_desc: '1533051063大安醫院本院',
    hosp_name: '大安醫院',
    hosp_class: '北區',
    hosp_city: '新竹縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '3',
    location: [24.8347547, 121.0081372]
  },
  {
    hosp_desc: '1533051072新仁醫院本院',
    hosp_name: '新仁醫院',
    hosp_class: '北區',
    hosp_city: '新竹縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.8356106, 121.0078548]
  },
  {
    hosp_desc: '1534050024杏和醫院本院',
    hosp_name: '杏和醫院',
    hosp_class: '台北區',
    hosp_city: '宜蘭縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '4',
    location: [24.8206314, 121.7691124]
  },
  {
    hosp_desc: '1535010024協和醫院本院',
    hosp_name: '協和醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '3',
    location: [24.5412601, 120.8174152]
  },
  {
    hosp_desc: '1535010051大千綜合醫院本院',
    hosp_name: '大千綜合醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '中度',
    hosp_ranking: '9',
    hosp_erbed: '27',
    location: [24.5498554, 120.8148874]
  },
  {
    hosp_desc: '1535010122大川醫院本院',
    hosp_name: '大川醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.5605684, 120.8221118]
  },
  {
    hosp_desc: '1535031041通霄光田醫院本院',
    hosp_name: '通霄光田醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '3',
    location: [24.4889662, 120.67846]
  },
  {
    hosp_desc: '1535040068慈祐醫院本院',
    hosp_name: '慈祐醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '3',
    location: [24.6848245, 120.8788289]
  },
  {
    hosp_desc: '1535040086大眾醫院本院',
    hosp_name: '大眾醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [24.6907853, 120.8795404]
  },
  {
    hosp_desc: '1535051178重光醫院本院',
    hosp_name: '重光醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '5',
    location: [24.686506, 120.908229]
  },
  {
    hosp_desc: '1535051196崇仁醫院本院',
    hosp_name: '崇仁醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.6836523, 120.9041648]
  },
  {
    hosp_desc: '1535081078大順醫院本院',
    hosp_name: '大順醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '2',
    location: [24.422495, 120.867676]
  },
  {
    hosp_desc: '1536010046豐安醫院本院',
    hosp_name: '豐安醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [24.2516279, 120.7206138]
  },
  {
    hosp_desc: '1536011276新惠生醫院本院',
    hosp_name: '新惠生醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.2582524, 120.7193461]
  },
  {
    hosp_desc: '1536011294祥恩醫院本院',
    hosp_name: '祥恩醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [24.2481506, 120.7176096]
  },
  {
    hosp_desc: '1536060037明德醫院本院',
    hosp_name: '明德醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [24.253839, 120.535773]
  },
  {
    hosp_desc: '1536061114忠港醫院本院',
    hosp_name: '忠港醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.252461, 120.536012]
  },
  {
    hosp_desc: '1536100081清泉醫院本院',
    hosp_name: '清泉醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '5',
    location: [24.2169347, 120.6663188]
  },
  {
    hosp_desc: '1536151042烏日澄清醫院本院',
    hosp_name: '烏日澄清醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [24.1112682, 120.6502033]
  },
  {
    hosp_desc: '1536181139本堂澄清醫院本院',
    hosp_name: '本堂澄清醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [24.0514849, 120.6947835]
  },
  {
    hosp_desc: '1536190076賢德醫院本院',
    hosp_name: '賢德醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '3',
    location: [24.1451957, 120.714181]
  },
  {
    hosp_desc: '1536200022達明眼科醫院本院',
    hosp_name: '達明眼科醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1113831, 120.6901157]
  },
  {
    hosp_desc: '1536201065新菩提醫院本院',
    hosp_name: '新菩提醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.115031, 120.688946]
  },
  {
    hosp_desc: '1537010040信生醫院本院',
    hosp_name: '信生醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.0858088, 120.5449468]
  },
  {
    hosp_desc: '1537010175冠華醫院本院',
    hosp_name: '冠華醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.0823919, 120.5428186]
  },
  {
    hosp_desc: '1537010219成美醫院本院',
    hosp_name: '成美醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.0831575, 120.5402849]
  },
  {
    hosp_desc: '1537010237順安醫院本院',
    hosp_name: '順安醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.0807465, 120.5439531]
  },
  {
    hosp_desc: '1537040057南星醫院本院',
    hosp_name: '南星醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.872507, 120.527754]
  },
  {
    hosp_desc: '1537040066卓醫院本院',
    hosp_name: '卓醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '3',
    location: [23.8720499, 120.5154723]
  },
  {
    hosp_desc: '1537050071員林何醫院本院',
    hosp_name: '員林何醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.957127, 120.570565]
  },
  {
    hosp_desc: '1537051274皓生醫院本院',
    hosp_name: '皓生醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.9591399, 120.573554]
  },
  {
    hosp_desc: '1537051292員生醫院本院',
    hosp_name: '員生醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '9',
    location: [23.959309, 120.566371]
  },
  {
    hosp_desc: '1537051309員郭醫院本院',
    hosp_name: '員郭醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.967372, 120.56244]
  },
  {
    hosp_desc: '1537061065道安醫院本院',
    hosp_name: '道安醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '3',
    location: [23.9615111, 120.4801972]
  },
  {
    hosp_desc: '1537070028仁和醫院本院',
    hosp_name: '仁和醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '3',
    location: [23.8580068, 120.5879024]
  },
  {
    hosp_desc: '1537081085宋志懿醫院本院',
    hosp_name: '宋志懿醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.9006701, 120.3729052]
  },
  {
    hosp_desc: '1537100012伸港忠孝醫院本院',
    hosp_name: '伸港忠孝醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1486528, 120.4858405]
  },
  {
    hosp_desc: '1538030037曾漢棋綜合醫院本院',
    hosp_name: '曾漢棋綜合醫院',
    hosp_class: '中區',
    hosp_city: '南投縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [23.9785896, 120.6857436]
  },
  {
    hosp_desc: '1538041101竹山秀傳醫院本院',
    hosp_name: '竹山秀傳醫院',
    hosp_class: '中區',
    hosp_city: '南投縣',
    hosp_injury: '中度',
    hosp_ranking: '6',
    hosp_erbed: '15',
    location: [23.8001648, 120.7139583]
  },
  {
    hosp_desc: '1538041165東華醫院本院',
    hosp_name: '東華醫院',
    hosp_class: '中區',
    hosp_city: '南投縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '5',
    location: [23.7642498, 120.7000345]
  },
  {
    hosp_desc: '1539010048洪揚醫院本院',
    hosp_name: '洪揚醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [23.7108506, 120.5488173]
  },
  {
    hosp_desc: '1539010057安生醫院本院',
    hosp_name: '安生醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.7083122, 120.5469659]
  },
  {
    hosp_desc: '1539040019育仁醫院本院',
    hosp_name: '育仁醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.801337, 120.4609143]
  },
  {
    hosp_desc: '1539050015蔡醫院本院',
    hosp_name: '蔡醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [23.6744327, 120.389765]
  },
  {
    hosp_desc: '1539060011全生醫院本院',
    hosp_name: '全生醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.5699399, 120.3015426]
  },
  {
    hosp_desc: '1539061063諸元內科醫院本院',
    hosp_name: '諸元內科醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.567108, 120.304324]
  },
  {
    hosp_desc: '1539061072北港仁一醫院本院',
    hosp_name: '北港仁一醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.5774706, 120.3012745]
  },
  {
    hosp_desc: '1541011126營新醫院本院',
    hosp_name: '營新醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '8',
    location: [23.3084846, 120.2998812]
  },
  {
    hosp_desc: '1541011162信一骨科醫院本院',
    hosp_name: '信一骨科醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.3064915, 120.3087691]
  },
  {
    hosp_desc: '1541031048佑昇醫院本院',
    hosp_name: '佑昇醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.3462032, 120.4111774]
  },
  {
    hosp_desc: '1541050016新生醫院本院',
    hosp_name: '新生醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.1603665, 120.1765272]
  },
  {
    hosp_desc: '1541070045宏科醫院本院',
    hosp_name: '宏科醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.127668, 120.287006]
  },
  {
    hosp_desc: '1542010052大東醫院本院',
    hosp_name: '大東醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '1',
    location: [22.6253849, 120.3619182]
  },
  {
    hosp_desc: '1542010141優生婦產科醫院本院',
    hosp_name: '優生婦產科醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6255889, 120.3509603]
  },
  {
    hosp_desc: '1542011237惠德醫院本院',
    hosp_name: '惠德醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '3',
    location: [22.5921035, 120.3251207]
  },
  {
    hosp_desc: '1542011246仁惠婦幼醫院本院',
    hosp_name: '仁惠婦幼醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.625853, 120.3533809]
  },
  {
    hosp_desc: '1542011282杏和醫院本院',
    hosp_name: '杏和醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '6',
    location: [22.5977253, 120.3356096]
  },
  {
    hosp_desc: '1542020058劉嘉修醫院本院',
    hosp_name: '劉嘉修醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.7957357, 120.2957629]
  },
  {
    hosp_desc: '1542020067光雄長安醫院本院',
    hosp_name: '光雄長安醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '8',
    location: [22.794195, 120.2959063]
  },
  {
    hosp_desc: '1542021171惠川醫院本院',
    hosp_name: '惠川醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '1',
    location: [22.7879803, 120.2967518]
  },
  {
    hosp_desc: '1542030018重安醫院本院',
    hosp_name: '重安醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.8830257, 120.484802]
  },
  {
    hosp_desc: '1542030116溪洲醫院本院',
    hosp_name: '溪洲醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.8928193, 120.4832034]
  },
  {
    hosp_desc: '1542040050三聖醫院本院',
    hosp_name: '三聖醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.8940591, 120.548117]
  },
  {
    hosp_desc: '1542050056建佑醫院本院',
    hosp_name: '建佑醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '5',
    location: [22.503781, 120.3866925]
  },
  {
    hosp_desc: '1542051151霖園醫院本院',
    hosp_name: '霖園醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '3',
    location: [22.5090379, 120.3946628]
  },
  {
    hosp_desc: '1542061077樂生婦幼醫院本院',
    hosp_name: '樂生婦幼醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6068691, 120.3956034]
  },
  {
    hosp_desc: '1542061148瑞生醫院本院',
    hosp_name: '瑞生醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '3',
    location: [22.61783, 120.3848748]
  },
  {
    hosp_desc: '1542110020泰和醫院本院',
    hosp_name: '泰和醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.7588644, 120.3096981]
  },
  {
    hosp_desc: '1542140046長佑醫院本院',
    hosp_name: '長佑醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.8775612, 120.3278239]
  },
  {
    hosp_desc: '1542150033溫賀睿和醫院本院',
    hosp_name: '溫賀睿和醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.8580536, 120.2586552]
  },
  {
    hosp_desc: '1542150042高新醫院本院',
    hosp_name: '高新醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [22.8654877, 120.2570872]
  },
  {
    hosp_desc: '1543010056復興醫院本院',
    hosp_name: '復興醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6685213, 120.4957531]
  },
  {
    hosp_desc: '1543010190民眾醫院本院',
    hosp_name: '民眾醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6764505, 120.4876058]
  },
  {
    hosp_desc: '1543020105茂隆骨科醫院本院',
    hosp_name: '茂隆骨科醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '2',
    location: [22.5562122, 120.5392631]
  },
  {
    hosp_desc: '1543110033大新醫院本院',
    hosp_name: '大新醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '1',
    location: [22.823793, 120.599612]
  },
  {
    hosp_desc: '0101090517臺北市立聯合醫院林森院區',
    hosp_name: '臺北市立聯合醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0634624, 121.5254748]
  },
  {
    hosp_desc: '0101090517臺北市立聯合醫院中醫昆明院區',
    hosp_name: '臺北市立聯合醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0441899, 121.504803]
  },
  {
    hosp_desc: '1231050017天主教耕莘醫療財團法人耕莘醫院安康院區',
    hosp_name: '天主教耕莘醫療財團法人耕莘醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '10',
    location: [24.9534219, 121.503908]
  },
  {
    hosp_desc: '0701160518臺北市立關渡醫院─委託臺北榮民總醫院經營本院',
    hosp_name: '臺北市立關渡醫院─委託臺北榮民總醫院經營',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '3',
    location: [25.1203139, 121.4661286]
  },
  {
    hosp_desc: '1501160042臺北市北投健康管理醫院健康中心',
    hosp_name: '臺北市北投健康管理醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.137558, 121.503699]
  },
  {
    hosp_desc: '0141270019衛生福利部胸腔病院大同院區',
    hosp_name: '衛生福利部胸腔病院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.987199, 120.2115986]
  },
  {
    hosp_desc: '0433050018國立臺灣大學醫學院附設醫院新竹生醫園區分院本院',
    hosp_name: '國立臺灣大學醫學院附設醫院新竹生醫園區分院',
    hosp_class: '北區',
    hosp_city: '新竹縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.8057313, 121.0446077]
  },
  {
    hosp_desc: '0634030014臺北榮民總醫院蘇澳分院附設門診部',
    hosp_name: '臺北榮民總醫院蘇澳分院',
    hosp_class: '台北區',
    hosp_city: '宜蘭縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.5962343, 121.8395583]
  },
  {
    hosp_desc: '0634070018臺北榮民總醫院員山分院附設員山鄉門診部',
    hosp_name: '臺北榮民總醫院員山分院',
    hosp_class: '台北區',
    hosp_city: '宜蘭縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.743456, 121.724085]
  },
  {
    hosp_desc: '0634070018臺北榮民總醫院員山分院附設門診部',
    hosp_name: '臺北榮民總醫院員山分院',
    hosp_class: '台北區',
    hosp_city: '宜蘭縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.7503463, 121.7576334]
  },
  {
    hosp_desc: '0638020014臺中榮民總醫院埔里分院暨南大學附設門診部',
    hosp_name: '臺中榮民總醫院埔里分院',
    hosp_class: '中區',
    hosp_city: '南投縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.9511431, 120.9306649]
  },
  {
    hosp_desc: '0646010013臺北榮民總醫院臺東分院勝利院區',
    hosp_name: '臺北榮民總醫院臺東分院',
    hosp_class: '東區',
    hosp_city: '臺東縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.7602711, 121.1587371]
  },
  {
    hosp_desc: '0905290020吉安醫療社團法人吉安醫院本院',
    hosp_name: '吉安醫療社團法人吉安醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.9630024, 120.3277851]
  },
  {
    hosp_desc: '0907120012燕巢靜和醫療社團法人燕巢靜和醫院本院',
    hosp_name: '燕巢靜和醫療社團法人燕巢靜和醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.7816252, 120.4164078]
  },
  {
    hosp_desc: '0911010010維德醫療社團法人基隆維德醫院本院',
    hosp_name: '維德醫療社團法人基隆維德醫院',
    hosp_class: '台北區',
    hosp_city: '基隆市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.130451, 121.799034]
  },
  {
    hosp_desc: '0912040012平和醫療社團法人和平醫院本院',
    hosp_name: '平和醫療社團法人和平醫院',
    hosp_class: '北區',
    hosp_city: '新竹市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.8088916, 120.9556374]
  },
  {
    hosp_desc: '0917050027維新醫療社團法人台中維新醫院本院',
    hosp_name: '維新醫療社團法人台中維新醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1571958, 120.6724348]
  },
  {
    hosp_desc: '0922020031祥太醫療社團法人祥太醫院本院',
    hosp_name: '祥太醫療社團法人祥太醫院',
    hosp_class: '南區',
    hosp_city: '嘉義市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.4760198, 120.4404253]
  },
  {
    hosp_desc: '0931050010怡濟慈園醫療社團法人宏濟神經精神科醫院本院',
    hosp_name: '怡濟慈園醫療社團法人宏濟神經精神科醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9590299, 121.513413]
  },
  {
    hosp_desc: '0931100015北新醫療社團法人北新醫院本院',
    hosp_name: '北新醫療社團法人北新醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.193765, 121.475468]
  },
  {
    hosp_desc: '0933010014培靈醫療社團法人關西醫院本院',
    hosp_name: '培靈醫療社團法人關西醫院',
    hosp_class: '北區',
    hosp_city: '新竹縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.7570301, 121.1899188]
  },
  {
    hosp_desc: '0934060027海天醫療社團法人海天醫院本院',
    hosp_name: '海天醫療社團法人海天醫院',
    hosp_class: '台北區',
    hosp_city: '宜蘭縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.7789321, 121.8106035]
  },
  {
    hosp_desc: '0935010021大千醫療社團法人南勢醫院本院',
    hosp_name: '大千醫療社團法人南勢醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.5193686, 120.7883288]
  },
  {
    hosp_desc: '0935020027李綜合醫療社團法人苑裡李綜合醫院中華院區',
    hosp_name: '李綜合醫療社團法人苑裡李綜合醫院',
    hosp_class: '北區',
    hosp_city: '苗栗縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.4380654, 120.6535646]
  },
  {
    hosp_desc: '0939010018信安醫療社團法人信安醫院本院',
    hosp_name: '信安醫療社團法人信安醫院',
    hosp_class: '南區',
    hosp_city: '雲林縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.6686594, 120.5113343]
  },
  {
    hosp_desc: '0941310023晉生醫療社團法人晉生慢性醫院本院',
    hosp_name: '晉生醫療社團法人晉生慢性醫院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.0214085, 120.2492746]
  },
  {
    hosp_desc: '0943060017屏安醫療社團法人屏安醫院本院',
    hosp_name: '屏安醫療社團法人屏安醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6708004, 120.567933]
  },
  {
    hosp_desc: '0943060017屏安醫療社團法人屏安醫院麟洛院區',
    hosp_name: '屏安醫療社團法人屏安醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6504952, 120.528008]
  },
  {
    hosp_desc: '1103280012醫療財團法人正德癌症醫療基金會佛教正德醫院本院',
    hosp_name: '醫療財團法人正德癌症醫療基金會佛教正德醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1435699, 120.6307911]
  },
  {
    hosp_desc: '1132071036長庚醫療財團法人桃園長庚紀念醫院長青院區',
    hosp_name: '長庚醫療財團法人桃園長庚紀念醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0151471, 121.368233]
  },
  {
    hosp_desc: '1134070019宜蘭員山醫療財團法人宜蘭員山醫院本院',
    hosp_name: '宜蘭員山醫療財團法人宜蘭員山醫院',
    hosp_class: '台北區',
    hosp_city: '宜蘭縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.721346, 121.721564]
  },
  {
    hosp_desc: '1137020520彰化基督教醫療財團法人鹿港基督教醫院長青院區',
    hosp_name: '彰化基督教醫療財團法人鹿港基督教醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.0552868, 120.468846]
  },
  {
    hosp_desc: '1138010019彰化基督教醫療財團法人南投基督教醫院本院',
    hosp_name: '彰化基督教醫療財團法人南投基督教醫院',
    hosp_class: '中區',
    hosp_city: '南投縣',
    hosp_injury: '一般',
    hosp_ranking: '2',
    hosp_erbed: '5',
    location: [23.8989152, 120.6839325]
  },
  {
    hosp_desc: '1143130019佑青醫療財團法人佑青醫院本院',
    hosp_name: '佑青醫療財團法人佑青醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.665878, 120.567644]
  },
  {
    hosp_desc: '1143150011迦樂醫療財團法人迦樂醫院本院',
    hosp_name: '迦樂醫療財團法人迦樂醫院',
    hosp_class: '高屏區',
    hosp_city: '屏東縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.4626117, 120.6039567]
  },
  {
    hosp_desc: '1145060029臺灣基督教門諾會醫療財團法人門諾醫院壽豐分院本院',
    hosp_name: '臺灣基督教門諾會醫療財團法人門諾醫院壽豐分院',
    hosp_class: '東區',
    hosp_city: '花蓮縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.8693864, 121.5257826]
  },
  {
    hosp_desc: '1401190039同仁院醫療財團法人萬華醫院本院',
    hosp_name: '同仁院醫療財團法人萬華醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0239974, 121.5098162]
  },
  {
    hosp_desc: '1417030017財團法人台灣省私立台中仁愛之家附設靜和醫院本院',
    hosp_name: '財團法人台灣省私立台中仁愛之家附設靜和醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1335909, 120.6594996]
  },
  {
    hosp_desc: '1417080517弘光科技大學附設老人醫院本院',
    hosp_name: '弘光科技大學附設老人醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1627049, 120.7228657]
  },
  {
    hosp_desc: '1431060017財團法人台灣省私立台北仁濟院附設新莊仁濟醫院本院',
    hosp_name: '財團法人台灣省私立台北仁濟院附設新莊仁濟醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.026948, 121.44273]
  },
  {
    hosp_desc: '1441060010財團法人台灣省私立台南仁愛之家附設精神療養院療養院',
    hosp_name: '財團法人台灣省私立台南仁愛之家附設精神療養院',
    hosp_class: '南區',
    hosp_city: '臺南市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.0511126, 120.3197562]
  },
  {
    hosp_desc: '1501010029培靈醫院本院',
    hosp_name: '培靈醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0494027, 121.5681918]
  },
  {
    hosp_desc: '1501101141泰安醫院本院',
    hosp_name: '泰安醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0614683, 121.5315551]
  },
  {
    hosp_desc: '1501201020景美醫院護理之家',
    hosp_name: '景美醫院',
    hosp_class: '台北區',
    hosp_city: '臺北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9911689, 121.539629]
  },
  {
    hosp_desc: '1502060041靜和醫院本院',
    hosp_name: '靜和醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.6360174, 120.3139737]
  },
  {
    hosp_desc: '1503030047美德醫院本院',
    hosp_name: '美德醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.3874125, 120.663746]
  },
  {
    hosp_desc: '1503250012宏恩醫院龍安分院本院',
    hosp_name: '宏恩醫院龍安分院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1194099, 120.6550624]
  },
  {
    hosp_desc: '1507300059維馨乳房外科醫院本院',
    hosp_name: '維馨乳房外科醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.667582, 120.3043505]
  },
  {
    hosp_desc: '1511060022南光神經精神科醫院本院',
    hosp_name: '南光神經精神科醫院',
    hosp_class: '台北區',
    hosp_city: '基隆市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.1383191, 121.7185206]
  },
  {
    hosp_desc: '1511060040暘基醫院本院',
    hosp_name: '暘基醫院',
    hosp_class: '台北區',
    hosp_city: '基隆市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.1396626, 121.712151]
  },
  {
    hosp_desc: '1522021237世華醫院本院',
    hosp_name: '世華醫院',
    hosp_class: '南區',
    hosp_city: '嘉義市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.4756624, 120.4420829]
  },
  {
    hosp_desc: '1531050086宏慈療養院療養院',
    hosp_name: '宏慈療養院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9374475, 121.4885312]
  },
  {
    hosp_desc: '1531060046大順醫院本院',
    hosp_name: '大順醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0357595, 121.4512035]
  },
  {
    hosp_desc: '1531060073新莊英仁醫院本院',
    hosp_name: '新莊英仁醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0357779, 121.453801]
  },
  {
    hosp_desc: '1531080226名恩療養院療養院',
    hosp_name: '名恩療養院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.9733097, 121.3249141]
  },
  {
    hosp_desc: '1531101113泓安醫院本院',
    hosp_name: '泓安醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.210801, 121.437825]
  },
  {
    hosp_desc: '1531140058全民醫院本院',
    hosp_name: '全民醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.0820688, 121.4757341]
  },
  {
    hosp_desc: '1531210019台安醫院本院',
    hosp_name: '台安醫院',
    hosp_class: '台北區',
    hosp_city: '新北市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.2084559, 121.4960465]
  },
  {
    hosp_desc: '1532060031居善醫院本院',
    hosp_name: '居善醫院',
    hosp_class: '北區',
    hosp_city: '桃園市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [25.058334, 121.166064]
  },
  {
    hosp_desc: '1536040535陽光精神科醫院本院',
    hosp_name: '陽光精神科醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.281092, 120.60931]
  },
  {
    hosp_desc: '1536040553清濱醫院本院',
    hosp_name: '清濱醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.2830846, 120.5489577]
  },
  {
    hosp_desc: '1536120010清海醫院本院',
    hosp_name: '清海醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.251125, 120.77419]
  },
  {
    hosp_desc: '1536190076賢德醫院樹孝',
    hosp_name: '賢德醫院',
    hosp_class: '中區',
    hosp_city: '臺中市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.1509675, 120.7167786]
  },
  {
    hosp_desc: '1537011243明德醫院本院',
    hosp_name: '明德醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [24.0816179, 120.552568]
  },
  {
    hosp_desc: '1537051265敦仁醫院本院',
    hosp_name: '敦仁醫院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.95053, 120.6029044]
  },
  {
    hosp_desc: '1537150512員林郭醫院大村分院本院',
    hosp_name: '員林郭醫院大村分院',
    hosp_class: '中區',
    hosp_city: '彰化縣',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [23.9990305, 120.5388819]
  },
  {
    hosp_desc: '1542020129樂安醫院本院',
    hosp_name: '樂安醫院',
    hosp_class: '高屏區',
    hosp_city: '高雄市',
    hosp_injury: '否',
    hosp_ranking: '1',
    hosp_erbed: '0',
    location: [22.7925228, 120.2812662]
  }
]
export default {
  components: {
    Alert
  },
  data () {
    return {
      data: Object.assign(DB_MC_PATIENT_INFO, {}),
      hospList,
      windowHeight: 0,
      windowWidth: 0
    }
  },
  mounted: function () {
    this.$nextTick(function () {
      window.addEventListener('resize', this.GetWindowHeight)
      window.addEventListener('resize', this.GtWindowWidth)
      window.addEventListener('scroll', this.handleScroll)
      // Init
      this.GetWindowHeight()
      this.GtWindowWidth()
    })
  },
  computed: {
    site_id () {
      if (this.$auth.getSiteID()) {
        return this.$auth.getSiteID()
      }
      return ''
    },
    selectPatList () {
      return this.$store.state.selectPatList
    },
    selectPatListLength () {
      return this.$store.state.selectPatList.length
    },
    patListLength () {
      return this.$store.state.patList.length
    },
    patList () {
      return this.$store.state.patList
    },
    title () {
      return document.title
    },
    now () {
      return this.$moment().format('YYYYMMDDHHmmss')
    }
  },
  watch: {
    titleName () {
      return document.title
    }
  },
  methods: {
    error (err) {
      if (typeof err === 'string') {
        this.$notification.error({
          message: err
        })
      } else {
        this.$notification.error({
          message: err.data
        })
      }
    },
    info (pVal) {
      if (typeof pVal === 'string') {
        this.$notification.info({
          message: pVal
        })
      }
    },
    FlattenObject (ob) {
      var toReturn = {}

      for (var i in ob) {
        if (!ob.hasOwnProperty(i)) continue

        if (typeof ob[i] === 'object' && ob[i] !== null) {
          var flatObject = this.FlattenObject(ob[i])
          for (var x in flatObject) {
            if (!flatObject.hasOwnProperty(x)) continue

            toReturn[i + '.' + x] = flatObject[x]
          }
        } else {
          toReturn[i] = ob[i]
        }
      }
      return toReturn
    },
    handleScroll () {
      this.scrolled = window.scrollY > 0
    },
    GetWindowHeight: function (event) {
      this.windowHeight = document.documentElement.clientHeight
      this.$store.commit({
        type: 'Basic/SetWindowHeight',
        windowWidth: document.documentElement.clientWidth
      })
    },
    GtWindowWidth: function (event) {
      var vuethis = this
      vuethis.contentTop = 0
      vuethis.windowWidth = document.documentElement.clientWidth
      vuethis.$store.commit({
        type: 'Basic/SetWindowWidth',
        windowWidth: document.documentElement.clientWidth
      })
    },
    SetNewModel () {
      return JSON.parse(JSON.stringify(DB_MC_PATIENT_INFO))
      //   return {
      //     datastatus: '0',
      //     PATIENT_ID: '',
      //     PATIENT_NAME: '',
      //     sex: '',
      //     ageType: '',
      //     TRIAGE: '',
      //     nation_type: '',
      //     start_date: this.$moment().format('YYYY-MM-DD HH:mm:ss'),
      //     amb_id: ''
      //   }
    },
    moment (dateString, format) {
      var _dateString = !!dateString
      return _dateString ? this.$moment(dateString, format) : null
    }
  },
  beforeDestroy: function () {
    window.removeEventListener('resize', this.GetWindowHeight)
    window.removeEventListener('resize', this.GtWindowWidth)
    window.removeEventListener('scroll', this.handleScroll)
  }
}
