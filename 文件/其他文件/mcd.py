###################################################
######傷患醫院選擇function##########################
####################################################
import random
import numpy as np
import pandas as pd

def penalty_standardize(cv,n_patient):
    p_min = 1
    p_max = cv
    s_min = 2
    s_max = 4
    if cv == 1:
        y = 2
    else:
        x = (s_max-s_min)/(p_max-p_min)
        b = s_min-(p_min*x)
        y = n_patient*x+b
    return y

def dist_ordinal(row,df):
    y = (df['dist'].max()+1)-row['dist']
    return y


def HpScore(severe, moderate, mild, dic_hp, penalty): 
    lt_hpscore_frt = []
    lt_hpscore = []
    # accumulate received casualties
    dic_npatient = dic_hp.fromkeys(list(dic_hp.keys()),0)
    dic_npatient_severe = dic_hp.fromkeys(list(dic_hp.keys()),0)
    dic_npatient_moderate = dic_hp.fromkeys(list(dic_hp.keys()),0)
    dic_npatient_mild = dic_hp.fromkeys(list(dic_hp.keys()),0)
    
    random.seed(0)
    n = severe+ moderate+ mild
    seq = ['severe']*severe+ ['moderate']*moderate+ ['mild']*mild
    sample = random.sample(seq, k=n)

    for i in range(n):
        severity = sample[i]
        dic_y = {}
        dic_yw = {}
        dic_z = {}
        dic_score = {}
        dic_penalty = {}
        for hospid in list(dic_hp.keys()):
            x = dic_hp[hospid]['distR'][0]
            w = dic_hp[hospid]['w2'][0]
            ad_edobv = dic_hp[hospid]['ad_edobservbeds'][0]
            penalty1 = penalty_standardize(dic_hp[hospid]['cv'][0],dic_npatient[hospid])  
            z = (penalty1*dic_npatient_severe[hospid]+
                 penalty*dic_npatient_moderate[hospid]+
                 penalty*dic_npatient_mild[hospid])/ad_edobv
            
            if severity == 'severe':
                y = dic_hp[hospid]['y_severe'][0]
                if dic_hp[hospid]['ECRHthreeLevel'][0]!=1:
                    score = x+y*(1-z)+y*w
                    
            elif severity == 'moderate':
                y = dic_hp[hospid]['y_moderate'][0]     
                score = x+y*(1-z)+y*w
                
            else:
                y = dic_hp[hospid]['y_mild'][0]    
                score = x+y*(1-z)+y*w
                
            dic_y[hospid] = y
            dic_yw[hospid] = y*w
            dic_z[hospid] = z
            dic_penalty[hospid] = penalty
            dic_score[hospid] = score    

        # hospital priority            
        hp_selected = sorted(dic_score, key=dic_score.get, reverse=True)[0]
#         print(hp_selected,dic_score[hp_selected])
        lt_hpscore_frt.append([i, severity, hp_selected,dic_hp[hp_selected]['key'][0], dic_npatient[hp_selected],\
                               dic_score[hp_selected], dic_hp[hp_selected]['ECRHthreeLevel'][0], dic_hp[hp_selected]['contype'][0],\
                               dic_hp[hp_selected]['dist'][0], dic_hp[hp_selected]['distR'][0],\
                               dic_y[hp_selected], dic_z[hp_selected], dic_hp[hp_selected]['w2'][0], dic_yw[hp_selected],\
                               dic_hp[hp_selected]['ad_edobservbeds'][0],\
                               dic_hp[hp_selected]['cv'][0]])
        cols = ['p_id','p_severity','hp_id','hp_key','p_accumulated','hp_score','hp_ecrhlevel','hp_contype',\
                'hp_dist','hp_distR','hp_scoreY','hp_scoreZ','w2','yw','hp_adedobservbeds','cv']
        outputfrt = pd.DataFrame(lt_hpscore_frt, columns=cols)
        
        # cumulative number of patients in selected hospital
        dic_npatient[hp_selected]+=1
        if severity == 'severe':
            dic_npatient_severe[hp_selected]+=1
        elif severity == 'moderate':
            dic_npatient_moderate[hp_selected]+=1
        else:
            dic_npatient_mild[hp_selected]+=1
        
        # sorted top 10 hospitals           
        for hp in sorted(dic_score, key=dic_score.get, reverse=True)[0:10]:
            lt_hpscore.append([i, severity, hp,dic_hp[hp]['key'][0], dic_npatient[hp], dic_score[hp],\
                                dic_hp[hp]['ECRHthreeLevel'][0], dic_hp[hp]['contype'][0],\
                               dic_hp[hp]['dist'][0], dic_hp[hp]['distR'][0],\
                               dic_y[hp], dic_z[hp], dic_hp[hp]['w2'][0], dic_yw[hp], dic_hp[hp]['ad_edobservbeds'][0],\
                               dic_hp[hp]['cv'][0]])
        output = pd.DataFrame(lt_hpscore, columns=cols)

    return outputfrt, output


###############################
###傷患醫院選擇執行#############
###############################
hp = pd.read_excel(r"D:\Anylogic\hospital_anylogic_20210208_py.xlsx")
# 收治醫院縣市別範圍 (由使用者選擇縣市)
hp = hp[(hp['mohwdivision'].isin(['台北區','北區']))&(~hp['city'].isin(['金門縣','連江縣','澎湖縣']))].reset_index(drop=True)
# 災害地點
hhp = hp.rename(columns = {'八仙':'dist'})
hp['distR'] = hp.apply(lambda x:dist_ordinal(x,hp),axis=1)
dic_hp = hp.groupby('id').apply(lambda dfg: dfg.drop('id', axis=1).to_dict(orient='list')).to_dict()
dic_hp_fix = dic_hp

writer = pd.ExcelWriter("./mcdscoretest/20210201-ffcde-mcd-pyV.xlsx", engine='xlsxwriter')
severe = 247
moderate = 159
mild = 89
penalty = 2
df_output = HpScore(severe, moderate, mild, dic_hp, penalty)
df_output[0].to_excel(writer, sheet_name='{}'.format('hp'),index=False)
df_output[1].to_excel(writer, sheet_name='{}'.format('allhp'),index=False)
writer.save()






###################################################
######資料前處理(以下可省略)##############################
####################################################

import numpy as np
import pandas as pd

# 205 ecrhs
hp546 = pd.read_excel(r"D:\佳欣的檔案\TMU Resilience\全台醫院統計\20210104-全台醫院病床數統計.xlsx",sheet_name='hp545v1')
hp205 = hp546[(hp546['緊急醫療能力分三級']!='否')&
              (hp546['醫院評鑑']!='兒童醫院評鑑合格')].reset_index(drop=True)

# ecrh level
hpecrh = pd.read_excel(r"D:\佳欣的檔案\TMU Resilience\全台醫院統計\20210104-全台醫院病床數統計.xlsx",sheet_name='ECRHlevel')
dic_hpecrh = hpecrh[['緊急醫療能力分級',
                     'Emergency Care Responsibility Hospital Level',
                     '緊急醫療能力分三級_ECRHthreeLevel','重傷', '中傷', '輕傷']]

# merge
hp205 = pd.merge(hp205,dic_hpecrh, on='緊急醫療能力分級', how = 'left')
hp205 = hp205.drop('緊急醫療能力分級',axis=1)

# ad_criticalbeds
hp205['調整後急重症相關病床數'] = hp205['急性一般病床']+hp205['加護病床']+hp205['調整後急診觀察床']+hp205['整合醫學急診後送病床']

hp205['內外科人力'] = hp205['外科']+hp205['急診醫學科']+hp205['麻醉科']+hp205['整形外科']+hp205['神經外科']+\
                  hp205['口腔病理科']+hp205['口腔顎面外科']+hp205['內科']+hp205['西醫一般科']+\
                  hp205['兒科']+hp205['放射診斷科']+hp205['放射腫瘤科']+hp205['神經科']+hp205['骨科']

# w2
hp205['w2'] = hp205['內外科人力'] / hp205['調整後急重症相關病床數']

# id
hp205['id'] = list(range(1,len(hp205)+1))


dic_col = {'機構名稱': 'hp_name',
          '縣市': 'city',
          '衛福部業務組別': 'mohwdivision',
          '特約別hospbsc': 'contype',
          '機構代碼機構名稱子院區':'key',
          '緊急醫療能力分三級_ECRHthreeLevel': 'ECRHthreeLevel', 
          '重傷':'y_severe',
          '中傷':'y_moderate',
          '輕傷':'y_mild',
          '急診觀察床': 'edobservbeds', 
          '調整後急診觀察床':'ad_edobservbeds',
          '調整後急重症相關病床數': 'ad_criticalbeds',
          '內外科人力':'medicalstaff'}


def critical_value(row):
    if row['ECRHthreeLevel']==3:
        # 急救責任醫院急診觀察床及調整後急診觀察床之Q1都是20
        # hp205v[hp205v['ECRHthreeLevel']==3]['edobservbeds'].describe(); Q1=20
        # hp205v[hp205v['ECRHthreeLevel']==3]['ad_edobservbeds'].describe(); Q1=20
        # 約每20張急診觀察床中有4張急救床可供所有傷勢傷患使用，即是醫院可收治傷患人數之臨界值。
        cv = row['edobservbeds']/20*4 # 設置急救室床位為4床
    else: cv = row['edobservbeds']
    return round(cv,0)

# the table for running distribution in python
hp205v = hp205[['id', '機構名稱', '縣市', '衛福部業務組別', 'Latitude', 'Longitude',
                '特約別hospbsc', '機構代碼機構名稱子院區', '緊急醫療能力分三級_ECRHthreeLevel',
                '重傷','中傷','輕傷',
                '急性一般病床','加護病床','急診觀察床','調整後急診觀察床',
                '整合醫學急診後送病床','調整後急重症相關病床數',
                '外科','急診醫學科','麻醉科','整形外科','神經外科',
                '口腔病理科','口腔顎面外科','內科','西醫一般科','兒科',
                '放射診斷科','放射腫瘤科','神經科','骨科','內外科人力','w2']]

# translation to English
hp205v = hp205v.rename(columns = dic_col)

# calculate criticle value
hp205v['cv'] = hp205v.apply(critical_value,axis=1)

# hp205v.to_excel(r"D:\Anylogic\hospital_anylogic_20210208_py.xlsx",index=False)
