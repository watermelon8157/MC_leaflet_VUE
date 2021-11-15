/*
【使用者介面UI】
收治醫院縣市別：基隆市、台北市、新北市、桃園市
救護人員數：100
EMT:100
醫護人員數：50
救護車數量：144
傷患傷勢：重傷247，中傷159，輕傷89
*/


/*
【Agent】
Agent: Hospital (hospitals)
D:/Anylogic/hospital_anylogic_20210208.xlsx

Agent: Patient
(預設為0，模擬開始進行時會依據條件產生資料)

Agent: Site (sites)
table name: disastersite
id, name, latitude, longitude
1, 八仙樂園, 25.142, 121.39
2, 普悠瑪, 24.646, 121.823
3, 迴龍捷運站, 25.03, 121.414
4, 大湖公園, 25.057, 121.359
5, 金山老街, 25.222, 121.638
6, 九份老街, 25.11, 121.845

//模擬八仙塵爆事件用不到此table資料，未來使用在多個隨機災害地點
table name: injuries (for example) 
siteid, type, timeinterval. nsevere, nmoderate, nmild, nneeddoc
1, 火災, 3, 40, 20, 35, 5
1, 火災, 2, 30, 15, 10, 3
*/


/*
【Function (不正規寫法)】

ecrhConv(int ecrhthreeLevel)
{
	if(ecrhthreeLevel==3){
	return "重度級";
	}
	else if(ecrhthreeLevel==2){
	return "中度級";
	}
	else{
	return "一般級";
	}
}

loadTab()
{
	List<Integer> lt_siteid = new ArrayList<Integer>();
	lt_siteid = selectFrom(injuries).list(injuries.siteid);

	List<Integer> lt_sitetype = new ArrayList<Integer>();
	lt_sitetype = selectFrom(injuries).list(injuries.type);

	List<Integer> lt_timeinterval = new ArrayList<Integer>();
	lt_timeinterval = selectFrom(injuries).list(injuries.timeinterval);

	List<Integer> lt_nsevere = new ArrayList<Integer>();
	lt_nsevere = selectFrom(injuries).list(injuries.nsevere);

	List<Integer> lt_nmoderate = new ArrayList<Integer>();
	lt_nmoderate = selectFrom(injuries).list(injuries.nmoderate);

	List<Integer> lt_nmild = new ArrayList<Integer>();
	lt_nmild = selectFrom(injuries).list(injuries.nmild);

	List<Integer> lt_nneeddoc = new ArrayList<Integer>();
	lt_nneeddoc = selectFrom(injuries).list(injuries.nneeddoc);
}

Setup(Patient agent, int siteid, int i)
{
	if(i == 1)
	{
		agent.DLevel =DLevel.L1;
		agent.severity = 1;
		agent.severityStr = "輕傷";
	}
	else if (i == 2)
	{
		agent.DLevel = DLevel.L2;
		agent.severity = 2;
		agent.severityStr = "中傷";
		
	}
	else
	{
		agent.DLevel = DLevel.L3;
		agent.severity = 3;
		agent.severityStr = "重傷";
		
	}
agent.siteid = siteid;
agent.lat = agent.getLatitude();
agent.lon = agent.getLongitude();	
}


penalty_standardize(int cv, int n_patient)
{
	// n_patient=1，penalty=2; 若1位傷患，懲罰項為2
	// cv, penalty=4
	double x = 0.0;
	double b = 0.0;
	if (cv==1){
		return 2;
	}
	else{
		x = (4-2)/(cv-1);
		b = 2-(1*x);
		return n_patient*x+b;
	}
}


HpScore(int max, Hospital h, int y)
{
	int penalty = 0;
	double score = 0.0;
	// siteid==1 八仙
	h.distanceR = max-h.mySites[siteid-1]+1;
	h.penalty1 = penalty_standardize(h.cv, h.n_patient);
	penalty = 2;
	h.z = (h.penalty1*h.n_severe+penalty*h.n_moderate+penalty*h.n_mild)/h.adEdobservbeds;
	score = h.distanceR+y*(1-h.z+h.w2);
	return score;
}
*/



/////////////On startup//////////////////////////////////////////////////////////
// 收治醫院縣市別範圍 (由使用者選擇縣市)
List<Hospital> filteredHps = new ArrayList<Hospital>();
List<String> cities = new ArrayList<String>();
for (int i = 0; i < cities.size(); i++){
	traceln("City = "+cities.get(i));
	String c = cities.get(i);
	filteredHps.addAll(filter(hospitals, h->h.city.equals(c)));
}



// multiple sites
// 計算所有災害地點到醫院的道路距離
for(Site s: sites){
	List<Integer> listSiteid = new ArrayList<Integer>();
	listSiteid.add(s.id);
	// s.id == 1 八仙樂園
	if (s.id == 1){
	    for(Hospital h: hospitals){
	    	 // 計算距離
			int dist = (int) Math.round((map.getDistanceByRoute(s.latitude, s.longitude, h.latitude, h.longitude))/100);
			h.mySites[s.id-1] = (int) Math.round(Math.pow( dist , 0.5 ));
			// convert int to String, such as 3 to "重傷"
			h.ecrhthreeLevelStr = ecrhConv(h.ecrhthreeLevel);
			// traceln("site:"+s.id+" distance:"+h.mySites[s.id-1]);
			int distpow = (int) Math.round(Math.pow( dist , 0.5 ));
			executeStatement("UPDATE hospital SET site = " + dist + " WHERE id = '" + h.id + "'");
			executeStatement("UPDATE hospital SET siteR = " + distpow + " WHERE id = '" + h.id + "'");
	   	}
	 }
}

// 使用者由UI輸入多個災害地點，不同災害地點不同的傷患人數
// 從table讀取資料
loadTab();	


int nsevere = 247;
int nmoderate = 159;
int nmild = 89;
ArrayList list = new ArrayList();
list.addAll(Collections.nCopies(nsevere,3));
list.addAll(Collections.nCopies(nmoderate,2));
list.addAll(Collections.nCopies(nmild,1));

////////////source///////////////////////////////////
// 給每位傷患賦予重傷、中傷、或輕傷
Random rand = new Random();
traceln("listsize:"+list.size());
traceln("listsize rand:"+rand.nextInt(list.size()));

int rands = (int) list.get(rand.nextInt(list.size()));
traceln(rands);
Setup(agent, siteid, rands);
list.remove(rands);


//////////queue1////////////////////////////////////
// severe
int max = 0;

for( Hospital h : filteredHps ) {
   if(h.ecrhthreeLevel!=1){
   	   max = sortDescending(hospitals, hm->hm.mySites[agent.siteid-1]).get(0).mySites[siteid-1];
	   h.score_severe = HpScore(max, h, h.ySevere);
	}
}
List<Hospital> sortHp_severe = new ArrayList<Hospital>()
sortHp_severe = sortDescending(hospitals, hd->hd.score_severe);

// moderate, mild
for( Hospital h : filteredHps ) {
	max = sortDescending(hospitals, hm->hm.mySites[agent.siteid-1]).get(0).mySites[siteid-1];
	h.score_moderate = HpScore(max, h, h.yModerate);
	h.score_mild = HpScore(max, h, h.yMild);
}
List<Hospital> sortHp_moderate = new ArrayList<Hospital>()
sortHp_moderate = sortDescending(hospitals, hd->hd.score_moderate);
List<Hospital> sortHp_mild = new ArrayList<Hospital>()
sortHp_mild = sortDescending(hospitals, hd->hd.score_mild);


if(agent.severity==3){
	
	//traceln("重傷");
	Hospital h = sortHp_severe.get(0);
	agent.destination= h;
	
	h.n_severe++;
	h.n_patient++;
	traceln(h.hpName);
	
	LogHosp(agent, h.id, h.key, 
	h.n_patient, h.score_severe, h.ecrhthreeLevel, 
	h.contype, h.mySites[agent.siteid-1], h.distanceR, 
	h.ySevere, h.z, h.w2, 
	h.edobservbeds, h.adEdobservbeds);
	
	
	for(int i=0; i<15; i++){
		Hospital hp = sortHp_severe.get(i);
		LogHosp5(agent, hp.id, hp.key, 
		hp.n_patient, hp.score_severe, hp.ecrhthreeLevel, 
		hp.contype, hp.mySites[agent.siteid-1], hp.distanceR, 
		hp.ySevere, hp.z, hp.w2, 
		hp.edobservbeds, hp.adEdobservbeds, i+1);
	}
}

else if(agent.severity==2){
	
	//traceln("中傷");	
	Hospital h = sortHp_moderate.get(0);
	agent.destination= h;
	
	h.n_moderate++;
	h.n_patient++;
	traceln(h.hpName);
	
	LogHosp(agent, h.id, h.key, 
	h.n_patient, h.score_moderate, h.ecrhthreeLevel, 
	h.contype, h.mySites[agent.siteid-1], h.distanceR, 
	h.yModerate, h.z, h.w2, 
	h.edobservbeds, h.adEdobservbeds);
	
	
	for(int i=0; i<5; i++){
		Hospital hp = sortHp_moderate.get(i);
		LogHosp5(agent, hp.id, hp.key, 
		hp.n_patient, hp.score_moderate, hp.ecrhthreeLevel, 
		hp.contype, hp.mySites[agent.siteid-1], hp.distanceR, 
		hp.yModerate, hp.z, hp.w2, 
		hp.edobservbeds, hp.adEdobservbeds, i+1);
	}
}

else{
	
	//traceln("輕傷");
	Hospital h = sortHp_mild.get(0);
	agent.destination= h;
	
	h.n_mild++;
	h.n_patient++;
	traceln(h.hpName);
	
	LogHosp(agent, h.id, h.key, 
	h.n_patient, h.score_mild, h.ecrhthreeLevel, 
	h.contype, h.mySites[agent.siteid-1], h.distanceR, 
	h.yMild, h.z, h.w2, 
	h.edobservbeds, h.adEdobservbeds);
	
	for(int i=0; i<5; i++){
		Hospital hp = sortHp_mild.get(i);
		LogHosp5(agent, hp.id, hp.key, 
		hp.n_patient, hp.score_mild, hp.ecrhthreeLevel, 
		hp.contype, hp.mySites[agent.siteid-1], hp.distanceR, 
		hp.yMild, hp.z, hp.w2, 
		hp.edobservbeds, hp.adEdobservbeds, i+1);
	}	
	
}



/////////////////////////////
/////以下可省略///////////////
/////////////////////////////

///////event//////////////////////////////
siteid = 1;
source.inject(495);
/*
int rand;
traceln(lt_sitetype.get(cnt_event));
switch( lt_sitetype.get(cnt_event) ) {
	case "火災":
	if(cnt_event<nsite){
		
		rand = uniform_discr(0,listSiteid.size()-1);
		traceln(listSiteid.size());
		traceln(listSiteid);
		traceln("rand:"+rand);
		//siteid = listSiteid.get(rand);
		siteid = 1;
		traceln("siteid:"+siteid);
		
		source.inject(lt_nsevere.get(cnt_event)+
					lt_nmoderate.get(cnt_event)+
					lt_nmild.get(cnt_event));
		
		//source.inject(DAmount);
		//listSiteid.remove(rand);
		cnt_event++;
		   
	}
	else {self.reset();}
	break;
}
*/

//////////batch: on add//////////////////
batch.set_siteid( agent.siteid );
batch.setLatLon( agent.lat, agent.lon );
batch.severityStr = agent.severityStr;
batch.severity = agent.severity;
//batch.destination = agent.destination;

batch.groupsize = agent.groupsize;
batch.groupsize = Amb_nmild;