using System.Collections;
using System.Collections.Generic;
using System.Text.RegularExpressions;
using UnityEngine;
using UnityEngine.UI;
using Spine;
using Spine.Unity;
using Unity.Notifications.Android;
using GoogleMobileAds.Api;
using System;

#pragma warning disable 0414

public class Status : MonoBehaviour
{

    public GameObject TouchBlock;

    // Spine Skeleton
    public SkeletonAnimation CreatureSkeleton;
    // 일단 여기다 선언해놓긴 하는데, unity inspector상에선 업데이트가 잘 안되니까, inspector에서 걍 수동으로 업데이트 할 것
    public SkeletonDataAsset[] skeletonDataAsset = new SkeletonDataAsset[7];
    public SkeletonDataAsset RIP;

    public Toggle notiAllowToggle;

    public GameObject AreYouSurePanel;
    public Text AreYouSureText;
    public GameObject evolutionButton;


    // ----------------------------------------------------------------------- Status controller

    float TryParseCheck;

    // XXX Value = negative 
    // XXX Tick = Positive
    // XXX To XXX = negative 
    // XXX To XXX Level = Positive

    // Status ---------------------------------

    public float status_hunger = 100f; // 'u'
    public float status_dirt = 100f; // 'd'

    public float status_happiness = 100f; // 'a' 'A'
    public float status_health = 100f; // 'e' 'E'

    // Hidden Status -------------------------

    public float status_loneiness = 0f; // 'L'
    public float status_goodness = 0f; // 'g'
    public float status_badness = 0f; // 'b'
    public float status_nightNday = 0f; // 'n'
    public float status_psysical = 0f; // 'p'
    public float status_intelligence = 0f; // 'I'

    //Min 기준으로 계산하도록 되어있음. 일단 2일
    public float status_revolutionTime = 2880f;


    float nullToHealth = 0f;
    float nullToHappiness = 0f;

    //------------------------------hunger
    float hungerValue = -2f;
    float hungerTick = 900f; // 15min

    int hungerStack = 0;
    int hungerDeadStack = 0;
    float hungerStackLevelTop = 70f;
    float hungerStackLevelBottom = 30f;

    float hungerToHappiness = -1f;
    float hungerToHappinessLevel = 50f;
    float hungerToHealth = -1f;
    float hungerToHealthLevel = 50f;

    //------------------------------dirt

    float dirtValue = -1f;
    float dirtTick = 900f;

    int dirtStack = 0;
    int dirtDeadStack = 0;
    float dirtStackLevelTop = 70f;
    float dirtStackLevelBottom = 30f;

    float dirtToHappiness = -1f;
    float dirtToHappinessLevel = 50f;
    float dirtToHealth = -1f;
    float dirtToHealthLevel = 50f;

    //-----------------------------happiness

    float happyValue = -1f;
    float happyTick = 900f;

    int happyStack = 0;
    int happyDeadStack = 0;
    float happyStackLevelTop = 70f;
    float happyStackLevelBottom = 30f;

    //-----------------------------health

    float healthValue = -1f;
    float healthTick = 900f;

    int healthStack = 0;
    int healthDeadStack = 0;
    float healthStackLevelTop = 70f;
    float healthStackLevelBottom = 30f;

    // Accessary -----------------------------

    // Effect의 범위 : 자연적으로 떨어지는 것, 아이템 사용으로 변하는 것, 다른 영향력에 영향을 행사하는 것
    // Effect는 곱연산, Effect끼리는 합연산

    // Status에 영향
    // => Increase 
    // EX > 차고있으면, hunger감소량이 20% 줄어듭니다. = accessary > hungerValue * 0.8%
    // EX > 차고있으면, 배고픔으로 인한 행복도 감소량이 20% 줄어듭니다. = accessary > hungerToHappiness * 0.8%

    // Compensators--------------------------- 
    // Tick의 경우, 결과적으로는 Value와 같기 때문에, 별도로 Compensator를 만들지 않음.

    // Hidden Status -------------------------

    public float loneiness_Compensator = 1f; // 'L'
    float goodness_Compensator = 1f; // 'g'
    float badness_Compensator = 1f; // 'b'
    float nightNday_Compensator = 1f; // 'n'
    float psysical_Compensator = 1f; // 'p'
    float intelligence_Compensator = 1f; // 'I'


    //------------------------------hunger
    float hungerValue_Compensator = 1f;

    float hungerToHappiness_Compensator = 1f;
    float hungerToHappinessLevel_Compensator = 1f;
    float hungerToHealth_Compensator = 1f;
    float hungerToHealthLevel_Compensator = 1f;

    float u_Compensator = 1f;

    //------------------------------dirt

    float dirtValue_Compensator = 1f;

    float dirtToHappiness_Compensator = 1f;
    float dirtToHappinessLevel_Compensator = 1f;
    float dirtToHealth_Compensator = 1f;
    float dirtToHealthLevel_Compensator = 1f;

    float d_Compensator = 1f;

    //-----------------------------happiness

    float happyValue_Compensator = 1f;

    float A_Compensator = 1f;

    //-----------------------------health

    float healthValue_Compensator = 1f;

    float E_Compensator = 1f;

    // 버튼 합산과정에 영향.  
    // => Increase By Char
    // EX > 차고있으면, 음식으로 인한 만복도 증가량이 10% 증가합니다. = buttonValue * 1.1%
    IEnumerator hunger;
    IEnumerator dirt;
    IEnumerator happiness;
    IEnumerator health;
    IEnumerator TimeSet;


    void Start()
    {
        //PlayerPrefs.DeleteAll();

        //앱을 진짜 처음 켰을때 부분. Perf에서 Key값을 검사해서, 없는 것들을 채워넣는다.
        ApplicationFirstStart();

        if (PlayerPrefs.GetInt("IndexNumber") != -1)
        {

            //시작할때 현재 단계에 맞추어 Creature 생성.
            CreateCreature(PlayerPrefs.GetInt("IndexNumber"));

            //Perf에서 저장된 Status 불러오기.
            StatusSynchronizerValueFromPerfs();

            //Compensater 설정
            CompensatorSynchronizer();

            // 켠 다음에 Status를 경과한 시간에 따라 reduce
            OnOffStatusSetter();

            // reduce가 끝나면 Status감소 Coroutine 시작
            StatusIncreaseSetup();

            // 처음 진화했을 때, LastTime을 한번 설정 해 주어야 함.
            PlayerPrefs.SetString("LastTime", System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm"));

            //AndroidNotificationCenter 세팅
            AndroidNotificationCenter.RegisterNotificationChannel(defaultNotificationChannel);
        }
        else 
        {

            touchBlocker.SetActive(true);

            CreateCreature(PlayerPrefs.GetInt("IndexNumber"));

            //Perf에서 저장된 Status 불러오기.
            StatusSynchronizerValueFromPerfs();

            //Compensater 설정
            CompensatorSynchronizer();

            // 처음 진화했을 때, LastTime을 한번 설정 해 주어야 함.
            PlayerPrefs.SetString("LastTime", System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm"));

        }


        // Option설정이랑 동기화;
        if (PlayerPrefs.GetInt("IsNotificatioAllow") == 1)
        { notiAllowToggle.isOn = true; }
        else { notiAllowToggle.isOn = false; }
    }

    // 처음 켰을때 부분.
    void ApplicationFirstStart()
    {
        // PlayerPerfs로 값을 읽는것들이 선언되어있지 않으면 에러나서 작동을 안하니까. 쭉 훑어가면서 선언해줌.
        if (!PlayerPrefs.HasKey("IndexNumber"))
        {
            PlayerPrefs.SetInt("IndexNumber", 0);

            //Dictionary
            PlayerPrefs.SetInt("Creature0", 1);

            statusResetter();
        }
        if (!PlayerPrefs.HasKey("LastTime")) { PlayerPrefs.SetString("LastTime", System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm")); }
        if (!PlayerPrefs.HasKey("Charm0")) { PlayerPrefs.SetInt("Charm0", 0); }
        if (!PlayerPrefs.HasKey("Charm1")) { PlayerPrefs.SetInt("Charm1", 0); }
        if (!PlayerPrefs.HasKey("Charm2")) { PlayerPrefs.SetInt("Charm2", 0); }
        if (!PlayerPrefs.HasKey("Stamina")) { PlayerPrefs.SetFloat("Stamina", 100f); PlayerPrefs.SetInt("StaminaSettime", 10); }
        if (!PlayerPrefs.HasKey("Coin")) { PlayerPrefs.SetInt("Coin", 50); }
        if (!PlayerPrefs.HasKey("ADTime")) { PlayerPrefs.SetString("ADTime", System.DateTime.Now.AddMinutes(-30).ToString("yyyy-MM-dd-HH-mm")); }

        // Option
        if (!PlayerPrefs.HasKey("IsNotificatioAllow")) { PlayerPrefs.SetInt("IsNotificatioAllow", 1); }
    }

    void statusResetter()
    {

        PlayerPrefs.SetString("StartTime", System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm"));

        PlayerPrefs.SetFloat("status_hunger", 100f); // 'u'
        PlayerPrefs.SetFloat("status_dirt", 100f); // 'd'

        PlayerPrefs.SetFloat("status_happiness", 100f); // 'a' 'A'
        PlayerPrefs.SetFloat("status_health", 100f); // 'e' 'E'

        PlayerPrefs.SetFloat("status_loneiness", 0f); // 'L'
        PlayerPrefs.SetFloat("status_goodness", 0f); // 'g'
        PlayerPrefs.SetFloat("status_badness", 0f); // 'b'
        PlayerPrefs.SetFloat("status_nightNday", 0f); // 'n'
        PlayerPrefs.SetFloat("status_psysical", 0f); // 'p'
        PlayerPrefs.SetFloat("status_intelligence", 0f); // 'I'

        PlayerPrefs.SetInt("hungerStack", 0);
        PlayerPrefs.SetInt("hungerDeadStack", 0);
        PlayerPrefs.SetInt("dirtStack", 0);
        PlayerPrefs.SetInt("dirtDeadStack", 0);
        PlayerPrefs.SetInt("happyStack", 0);
        PlayerPrefs.SetInt("happyDeadStack", 0);
        PlayerPrefs.SetInt("healthStack", 0);
        PlayerPrefs.SetInt("healthDeadStack", 0);

    }

    //끈시간 켠시간을 비교해서, Status를 조정함

    public GameObject OnOffChecker;
    public Text onoffText_Time;
    public Text onoffText;

    void OnOffStatusSetter() {

        float hungerCheck = 0;
        float dirtCheck = 0;
        float happyCheck = 0;
        float healthCheck = 0;

        float LastTime = 0f;
        float OnTime = 0f;
        TimeToNumber(PlayerPrefs.GetString("LastTime"), ref LastTime);
        TimeToNumber(System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm"), ref OnTime);

        float StatusTickCounter = 0f;

        //10분이상 차이나야 작동하도록 설정했음. 
        if (OnTime - LastTime > 10f) {
            //hunger
            StatusTickCounter = ((OnTime - LastTime) * 60f) / hungerTick;
            for (int i = 1; i <= StatusTickCounter; i++)
            { StatusIncreaserByChar('u', hungerValue.ToString()); hungerCheck += hungerValue; }

            //dirt
            StatusTickCounter = ((OnTime - LastTime) * 60f) / dirtTick;
            for (int i = 1; i <= StatusTickCounter; i++)
            { StatusIncreaserByChar('d', dirtValue.ToString()); dirtCheck += dirtValue; }

            //happy 
            StatusTickCounter = ((OnTime - LastTime) * 60f) / happyTick;
            for (int i = 1; i <= StatusTickCounter; i++)
            { StatusIncreaserByChar('a', happyValue.ToString()); happyCheck += happyValue; }

            //health
            StatusTickCounter = ((OnTime - LastTime) * 60f) / healthTick;
            for (int i = 1; i <= StatusTickCounter; i++)
            { StatusIncreaserByChar('e', healthValue.ToString()); healthCheck += healthValue; }

            //4시간이상 방치시, 방치된 시간만큼 loniness 상승
            if (OnTime - LastTime > 240f) { status_loneiness += ((OnTime - LastTime) / 240); }


            onoffText_Time.text = (OnTime - LastTime).ToString() + "분 만에 다시 오셨군요";

            onoffText.text = "당신의 달팽이는 \n\n";

            if (hungerCheck < -5) { onoffText.text += "조금 배고파졌습니다 \n"; } 
            else if (hungerCheck < -30) { onoffText.text += "배고파졌습니다 \n"; }
            else if (hungerCheck < -70) { onoffText.text += "많이 배고파졌습니다 \n"; }

            if (dirtCheck < -5) { onoffText.text += "조금 더러워졌습니다 \n"; }
            else if (dirtCheck < -30) { onoffText.text += "더러워졌습니다 \n"; }
            else if (dirtCheck < -70) { onoffText.text += "많이 더러워졌습니다 \n"; }

            if (happyCheck < -5) { onoffText.text += "조금 슬퍼졌습니다 \n"; }
            else if (happyCheck < -30) { onoffText.text += "슬퍼졌습니다 \n"; }
            else if (happyCheck < -70) { onoffText.text += "많이 슬퍼졌습니다 \n"; }

            if (healthCheck < -5) { onoffText.text += "조금 약해졌습니다 \n"; }
            else if (healthCheck < -30) { onoffText.text += "약해졌습니다 \n"; }
            else if (healthCheck < -70) { onoffText.text += "많이 약해졌습니다 \n"; }

            if(hungerStack + dirtStack + healthStack + happyStack > 40) { onoffText.text += "죽어가고 있어요!"; }

            if (onoffText.text == "당신의 달팽이는 \n\n"){ onoffText.text += "\n\n 별일 없었습니다"; }


            //밥을 너무 안주거나 하면 죽음.
            if (hungerStack + dirtStack + healthStack + happyStack > 60) { 
            
                Dead();
                onoffText.text = "저런..! 달팽이가 지쳐서 죽었습니다.."; 
                
            }
            //3일 이상 접속을 안하면 죽음.
            else if (OnTime - LastTime > 4320f) {
            
                Dead();
                onoffText.text = "저런..! 달팽이가 외로워서 죽었습니다..";    
            }

            OnOffChecker.SetActive(true);

        }

        for (int i = 1; i <= (OnTime - LastTime); i++) {
            // 1분마다 Stamina Increase Check
            StaminaSettime();

            //Debug.Log((OnTime - LastTime).ToString() + "에 의하여, StaminaSettime을 실행합니다. 현재 stamina " + PlayerPrefs.GetInt("stamina").ToString());

        }
    }

    public void _onOffPanelOff(){

        OnOffChecker.SetActive(false);

    }


    void StatusIncreaseSetup()
    {
        try
        {
            //하기전에 일단 모든 코루틴을 멈춘다. 
            //Start에도 실행할건데, 이때는 Corutine선언이 안되어있으니까 try로 error흘려보냄.
            StopCoroutine(hunger);
            StopCoroutine(dirt);
            StopCoroutine(happiness);
            StopCoroutine(health);
            StopCoroutine(TimeSet);
        }
        catch { }

        hunger = StatusReduce('u', status_hunger, hungerValue, hungerTick);
        dirt = StatusReduce('d', status_dirt, dirtValue, dirtTick);
        happiness = StatusReduce('a', status_happiness, happyValue, happyTick);
        health = StatusReduce('e', status_health, healthValue, healthTick);
        StartCoroutine(hunger);
        StartCoroutine(dirt);
        StartCoroutine(happiness);
        StartCoroutine(health);

        TimeSet = TimeSetter();
        StartCoroutine(TimeSet);

        Debug.Log("setupComplete");
    }

    //켜자마자 LastTime을 할당하고, 1분 단위로 갱신함. StartTime과의 시간을 비교해서 진화함.  
    IEnumerator TimeSetter() {

        float StartTime = 0f;
        float LastTime = 0f;
        TimeToNumber(PlayerPrefs.GetString("StartTime"), ref StartTime);

        while (true) {
            PlayerPrefs.SetString("LastTime", System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm"));
            TimeToNumber(PlayerPrefs.GetString("LastTime"), ref LastTime);

            if (LastTime - StartTime > status_revolutionTime)
            {
                evolutionButton.SetActive(true);
                //DoEvolution();
                //바로 진화하는게 아니라, 진화 버튼을 만들어야하네
            }

            if (PlayerPrefs.GetInt("IsNotificatioAllow") == 1)
            {
                //LastTime을 갱신할때마다 알람을 갱신함.
                _NotificationSetup();
            }

            Debug.Log((LastTime - StartTime).ToString());

            //Stamina 줄어드는 매커니즘
            StaminaSettime();

            //켜면 일단 한번 저장
            yield return new WaitForSeconds(60f);
        }
    }

    public void _notificationAllow(){
        
        if(notiAllowToggle.isOn){
            PlayerPrefs.SetInt("IsNotificatioAllow", 1);
            _NotificationSetup();
        }
        else{
            PlayerPrefs.SetInt("IsNotificatioAllow", 0);
            AndroidNotificationCenter.CancelAllNotifications();
        }
        

    }


    AndroidNotificationChannel defaultNotificationChannel = new AndroidNotificationChannel()
    {
        Id = "channel_id",
        Name = "Default Channel",
        Importance = Importance.High,
        Description = "Generic notifications",
    };

    AndroidNotification notification_hunger = new AndroidNotification();
    AndroidNotification notification_dirt = new AndroidNotification();
    AndroidNotification notification_missing = new AndroidNotification();

    public void _NotificationSetup()
    {
        double hungerTime = 0;
        double dirtTime = 0;

        if (status_hunger - 30f >= 0)
        { hungerTime = (((status_hunger - 30f) / (hungerValue * -1)) * hungerTick) + 1; }
        else if (status_hunger - 30f < 0)
        { hungerTime = (((status_hunger) / (hungerValue * -1)) * hungerTick) + 1; }

        if (status_dirt - 30f >= 0)
        { dirtTime = (((status_dirt - 30f) / (dirtValue * -1)) * dirtTick) + 1; }
        else if (status_dirt - 30f < 0)
        { dirtTime = (((status_dirt) / (dirtValue * -1)) * hungerTick) + 1; }

        notification_hunger.Title = "Creatures";
        notification_hunger.Text = "달팽이가 배고파하고 있습니다!";
        notification_hunger.FireTime = System.DateTime.Now.AddSeconds(hungerTime);
        notification_hunger.LargeIcon = "icon_0";
        notification_hunger.SmallIcon = "icon_1";

        notification_dirt.Title = "Creatures";
        notification_dirt.Text = "달팽이 집이 더러워졌어요!";
        notification_dirt.FireTime = System.DateTime.Now.AddSeconds(dirtTime);
        notification_dirt.LargeIcon = "icon_0";
        notification_dirt.SmallIcon = "icon_1";

        notification_missing.Title = "Creatures";
        notification_missing.Text = "달팽이가 당신을 보고싶어합니다.";
        notification_missing.FireTime = System.DateTime.Now.AddDays(1);
        notification_missing.LargeIcon = "icon_0";
        notification_missing.SmallIcon = "icon_1";

        AndroidNotificationCenter.CancelAllNotifications();
        AndroidNotificationCenter.SendNotification(notification_hunger, "channel_id");
        AndroidNotificationCenter.SendNotification(notification_dirt, "channel_id");
        AndroidNotificationCenter.SendNotification(notification_missing, "channel_id");
    }

    public GameObject touchBlocker;

    void Dead() {

        //PlayerPrefs.SetInt("IndexNumber" ..  를 -1 로 만듬

        CreateCreature(-1);
        statusResetter();
        StatusSynchronizerValueFromPerfs();

        touchBlocker.SetActive(true);

        try
        {
            // 코루틴을 다 멈춤
            StopCoroutine(hunger);
            StopCoroutine(dirt);
            StopCoroutine(happiness);
            StopCoroutine(health);
            StopCoroutine(TimeSet);
        }
        catch { }

    }

    public void _AreYouSureEvolution(){

        evolutionButton.SetActive(false);
        AreYouSurePanel.SetActive(false);
        DoEvolution();

    }

    public void _DoDeadButton(){
        Dead();
    }
    
    // 디버그용 진화버튼
    public void _EvolutionButton(){

        AreYouSurePanel.SetActive(true);
        AreYouSureText.text = "달팽이를 진화시켜볼까요?";
        //DoEvolution();
    }

    void DoEvolution() {

        // 진화 프로토콜 시작.
        // L, g, b, n, p, I 
        // loney , good, bad, night and day, psycial, intelligence

        // 진화조건을 어떻게 맞추지..?? 일단 임의로 만드는데 어떻게 해야하지
        // CreateCreature(PlayerPrefs.GetInt("IndexNumber"));
        // 일단 indexnumber로 하고, 나중에 짜지는대로 다시할 것


        // 2020-03-22 Temp Secter
        // 지금은 그냥 변하는데, 효과넣고 하면 효과가 끝날때 > Create하면 될 듯.

        

        if ( PlayerPrefs.GetInt("IndexNumber")  == -1) 
        {
            CreateCreature(0);
            //되살아 났습니다.

            touchBlocker.SetActive(false);
        }

        else 
        {
            int index = UnityEngine.Random.Range(0, skeletonDataAsset.Length);
            while (index == PlayerPrefs.GetInt("IndexNumber"))
            { index = UnityEngine.Random.Range(0, skeletonDataAsset.Length); }

            //Dictionary 체크
            PlayerPrefs.SetInt("Creature" + index.ToString(), 1);

            CreateCreature( index );
            Debug.Log(index.ToString() + "번째 달팽이로 진화했습니다.");
        }


        //코인 증가.
        _CoinIncrease(50);

        // Skeleton이 교체된 이후엔, 스테이터스와 시간을 초기화함.
        statusResetter();
        // 모든 코루틴을 중단하고, 변화된 스테이터스에 따라 초기화.
        StatusIncreaseSetup();
    }

    public void _CoinIncrease(int number){
        try { PlayerPrefs.SetInt("Coin", PlayerPrefs.GetInt("Coin") + number); }
        catch { }
    }

    void CreateCreature( int IndexNumber )
    {

        PlayerPrefs.SetInt("IndexNumber", IndexNumber);
        CreateSpine(IndexNumber);

        if ( IndexNumber >= 0)
        {
            Debug.Log("NowCreature IS : " + IndexNumber.ToString());

            // 할당된 IndexNumber를 바탕으로 new SpineSkeleton를 생성. 
            
            setStatus(IndexNumber);
        }
        else 
        {
            Debug.Log("Creature Is Dead..");
        }

    }

    public void CreateSpine(int index)
    {
        //Creature를 일단 삭제
        Destroy(GameObject.Find("Creature"));
        
        //Index에서 skeleton을 찾아서 새 Object를 만들고 ( = 오른쪽만으로 충분함) CreatureSkeleton 에 할당함
        //위치와 이름 애니메이션 설정

        if (index < 0)
        { 
            CreatureSkeleton = SkeletonAnimation.NewSkeletonAnimationGameObject(RIP);
            CreatureSkeleton.transform.Translate(new Vector3(-0.3f, 1.3f, 0f));
        }
        else 
        { 
            CreatureSkeleton = SkeletonAnimation.NewSkeletonAnimationGameObject(skeletonDataAsset[index]);
            CreatureSkeleton.transform.Translate(new Vector3(0f, 0.9f, 0f));
        }
        
        CreatureSkeleton.name = "Creature";
        CreatureSkeleton.AnimationState.SetAnimation(0, "stand", true).TimeScale = 1f;
    }

    void setStatus(int indexNumber)
    {
    
        List<Dictionary<string, object>> data = CSVReader.Read("TempCaracter/SkeletonStatus");
        
        status_revolutionTime = float.Parse(data[indexNumber]["LT"].ToString());

        //------------------------------hunger
        hungerValue = float.Parse(data[indexNumber]["hV"].ToString());
        hungerTick = float.Parse(data[indexNumber]["hT"].ToString());

        hungerStackLevelTop = float.Parse(data[indexNumber]["hSLT"].ToString());
        hungerStackLevelBottom = float.Parse(data[indexNumber]["hSLB"].ToString());

        hungerToHappiness = float.Parse(data[indexNumber]["hTA"].ToString());
        hungerToHappinessLevel = float.Parse(data[indexNumber]["hTAL"].ToString());
        hungerToHealth = float.Parse(data[indexNumber]["hTE"].ToString());
        hungerToHealthLevel = float.Parse(data[indexNumber]["hTEL"].ToString());

        //------------------------------dirt

        dirtValue = float.Parse(data[indexNumber]["dV"].ToString());
        dirtTick = float.Parse(data[indexNumber]["dT"].ToString());

        dirtStackLevelTop = float.Parse(data[indexNumber]["dSLT"].ToString());
        dirtStackLevelBottom = float.Parse(data[indexNumber]["dSLB"].ToString());

        dirtToHappiness = float.Parse(data[indexNumber]["dTA"].ToString());
        dirtToHappinessLevel = float.Parse(data[indexNumber]["dTAL"].ToString());
        dirtToHealth = float.Parse(data[indexNumber]["dTE"].ToString());
        dirtToHealthLevel = float.Parse(data[indexNumber]["dTEL"].ToString());

        //-----------------------------happiness

        happyValue = float.Parse(data[indexNumber]["aV"].ToString());
        happyTick = float.Parse(data[indexNumber]["aT"].ToString());

        happyStackLevelTop = float.Parse(data[indexNumber]["aSLT"].ToString());
        happyStackLevelBottom = float.Parse(data[indexNumber]["aSLB"].ToString());

        //-----------------------------health

        healthValue = float.Parse(data[indexNumber]["eV"].ToString());
        healthTick = float.Parse(data[indexNumber]["eT"].ToString());

        healthStackLevelTop = float.Parse(data[indexNumber]["eSLT"].ToString());
        healthStackLevelBottom = float.Parse(data[indexNumber]["eSLB"].ToString());

        /*
        PlayerPrefs.SetFloat("status_revolutionTime", float.Parse(data[indexNumber]["LT"].ToString()));

        PlayerPrefs.SetFloat("hungerValue", float.Parse(data[indexNumber]["hV"].ToString()));
        PlayerPrefs.SetFloat("hungerTick", float.Parse(data[indexNumber]["hT"].ToString()));

        PlayerPrefs.SetFloat("hungerStackLevelTop", float.Parse(data[indexNumber]["hSLT"].ToString()));
        PlayerPrefs.SetFloat("hungerStackLevelBottom", float.Parse(data[indexNumber]["hSLB"].ToString()));

        PlayerPrefs.SetFloat("hungerToHappiness", float.Parse(data[indexNumber]["hTA"].ToString()));
        PlayerPrefs.SetFloat("hungerToHappinessLevel", float.Parse(data[indexNumber]["hTAL"].ToString()));
        PlayerPrefs.SetFloat("hungerToHealth", float.Parse(data[indexNumber]["hTE"].ToString()));
        PlayerPrefs.SetFloat("hungerToHealthLevel", float.Parse(data[indexNumber]["hTEL"].ToString()));

        //------------------------------dirt

        PlayerPrefs.SetFloat("dirtValue", float.Parse(data[indexNumber]["dV"].ToString()));
        PlayerPrefs.SetFloat("dirtTick", float.Parse(data[indexNumber]["dT"].ToString()));

        PlayerPrefs.SetFloat("dirtStackLevelTop", float.Parse(data[indexNumber]["dSLT"].ToString()));
        PlayerPrefs.SetFloat("dirtStackLevelBottom", float.Parse(data[indexNumber]["dSLB"].ToString()));

        PlayerPrefs.SetFloat("dirtToHappiness", float.Parse(data[indexNumber]["dTA"].ToString()));
        PlayerPrefs.SetFloat("dirtToHappinessLevel", float.Parse(data[indexNumber]["dTAL"].ToString()));
        PlayerPrefs.SetFloat("dirtToHealth", float.Parse(data[indexNumber]["dTE"].ToString()));
        PlayerPrefs.SetFloat("dirtToHealthLevel", float.Parse(data[indexNumber]["dTEL"].ToString()));

        //-----------------------------happiness

        PlayerPrefs.SetFloat("happyValue", float.Parse(data[indexNumber]["aV"].ToString()));
        PlayerPrefs.SetFloat("happyTick", float.Parse(data[indexNumber]["aT"].ToString()));

        PlayerPrefs.SetFloat("happyStackLevelTop", float.Parse(data[indexNumber]["aSLT"].ToString()));
        PlayerPrefs.SetFloat("happyStackLevelBottom", float.Parse(data[indexNumber]["aSLB"].ToString()));

        //-----------------------------health

        PlayerPrefs.SetFloat("healthValue", float.Parse(data[indexNumber]["eV"].ToString()));
        PlayerPrefs.SetFloat("healthTick", float.Parse(data[indexNumber]["eT"].ToString()));

        PlayerPrefs.SetFloat("healthStackLevelTop", float.Parse(data[indexNumber]["eSLT"].ToString()));
        PlayerPrefs.SetFloat("healthStackLevelBottom", float.Parse(data[indexNumber]["eSLB"].ToString()));


        hungerValue = PlayerPrefs.GetFloat("hungerValue");
        hungerTick = PlayerPrefs.GetFloat("hungerTick");

        hungerStackLevelTop = PlayerPrefs.GetFloat("hungerStackLevelTop");
        hungerStackLevelBottom = PlayerPrefs.GetFloat("hungerStackLevelBottom");

        hungerToHappiness = PlayerPrefs.GetFloat("hungerToHappiness");
        hungerToHappinessLevel = PlayerPrefs.GetFloat("hungerToHappinessLevel");
        hungerToHealth = PlayerPrefs.GetFloat("hungerToHealth");
        hungerToHealthLevel = PlayerPrefs.GetFloat("hungerToHealthLevel");

        //------------------------------dirt

        dirtValue = PlayerPrefs.GetFloat("dirtValue");
        dirtTick = PlayerPrefs.GetFloat("dirtTick");

        dirtStackLevelTop = PlayerPrefs.GetFloat("dirtStackLevelTop");
        dirtStackLevelBottom = PlayerPrefs.GetFloat("dirtStackLevelBottom");

        dirtToHappiness = PlayerPrefs.GetFloat("dirtToHappiness");
        dirtToHappinessLevel = PlayerPrefs.GetFloat("dirtToHappinessLevel");
        dirtToHealth = PlayerPrefs.GetFloat("dirtToHealth");
        dirtToHealthLevel = PlayerPrefs.GetFloat("dirtToHealthLevel");

        //-----------------------------happiness

        happyValue = PlayerPrefs.GetFloat("happyValue");
        happyTick = PlayerPrefs.GetFloat("happyTick");

        happyStackLevelTop = PlayerPrefs.GetFloat("happyStackLevelTop");
        happyStackLevelBottom = PlayerPrefs.GetFloat("happyStackLevelBottom");

        //-----------------------------health

        healthValue = PlayerPrefs.GetFloat("healthValue");
        healthTick = PlayerPrefs.GetFloat("healthTick");

        healthStackLevelTop = PlayerPrefs.GetFloat("healthStackLevelTop");
        healthStackLevelBottom = PlayerPrefs.GetFloat("healthStackLevelBottom");
        */

        Debug.Log(data[indexNumber]["name"].ToString() + " StatusUpdateComplete");
    }


    float TimeToNumber(string yyyy_MM_dd_HH_mm, ref float Number){

        int Counter = 0;
        Number = 0f;

        foreach (char n in yyyy_MM_dd_HH_mm) {

            //더 세련되게 적을 수 있는 방법이 있다는걸 알긴 하는데., 그럼 나중에 못고칠듯 
            // yyyy _ MM _ dd _  H H _ mm
            // 0123 4 56 7 89 10 1112  1314

            if (Counter == 4 || Counter == 7 || Counter == 10 || Counter == 13) { /* DoNothing */ }
            else if (Counter == 0)
            { Number += int.Parse(n.ToString()) * 8760f * 60f; } //Year
            else if (Counter == 1)
            { Number += int.Parse(n.ToString()) * 8760f * 60f; }
            else if (Counter == 2)
            { Number += int.Parse(n.ToString()) * 8760f * 60f; }
            else if (Counter == 3)
            { Number += int.Parse(n.ToString()) * 8760f * 60f; }

            else if (Counter == 5)
            { Number += int.Parse(n.ToString()) * 10f * 732f * 60f; } //Month
            else if (Counter == 6)
            { Number += int.Parse(n.ToString()) * 732f * 60f; } //Month

            else if (Counter == 8)
            { Number += int.Parse(n.ToString()) * 10f * 24f * 60f; } //Day
            else if (Counter == 9)
            { Number += int.Parse(n.ToString()) * 24f * 60f; } 


            else if (Counter == 11)
            { Number += int.Parse(n.ToString()) * 10f * 60f; } //Hour
            else if (Counter == 12)
            { Number += int.Parse(n.ToString()) * 60f; } 

            else if (Counter == 14)
            { Number += int.Parse(n.ToString()) * 10f; } //Minuate
            else if (Counter == 15)
            { Number += int.Parse(n.ToString()); }

            Counter++;
        }
        
        // Debug.Log(Number.ToString());
        return Number;
    }


    //그냥 서있으면 감소하는 기본 감소량
    IEnumerator StatusReduce(char name, float Status, float Value, float Tick){
        
        //한번 쉬고 (켜자마자 스탯이 깎이는걸 : 껐다켰다하면 급속도로 스탯이 떨어지는 것 방지)
        yield return new WaitForSeconds(Tick);

        while (true){
            if(Status > 0) 
            { 
                StatusIncreaserByChar(name, Value.ToString());
                
                switch(name){
                    case 'u' : { Status = status_hunger; break; }
                    case 'd' : { Status = status_dirt;   break; }
                    case 'a' : { Status = status_happiness; break; }
                    case 'e' : { Status = status_health; break; }
                }
            }

            yield return new WaitForSeconds(Tick);
        }
    }
 
    // a00b00c00의 형태로 "이름" + "변경값"으로 되어있음. 0 ~ 50 증가, 51~99 감소
    public void _StatusChangeButton (string name){

        string charStack = null;
        bool ready = false;
        bool did = false;
        char changeStatName = ' ';
       
        //Button에 있는 String을 참조하여 Status을 변경함.
        foreach(char n in name){

            if( did ) { 
                charStack += n;
                ready = false;
                did = false;
                
                //ChangeButton을 할때 Char에는 대문자를 넣을 것
                StatusIncreaserByChar(changeStatName, charStack);
                Debug.Log(changeStatName + " Changed by : " + charStack);
                charStack = null;
            }

            if( ready && !did ) {
                charStack += n;
                did = true;
            }

            if (!float.TryParse(n.ToString(), out TryParseCheck)){
                ready = true; changeStatName = n;
            }
        }
    }

    //특정 스테이터스만 바꾸기, 변경값은 초기화 x 수동으로 초기화해야함
    void StatusIncreaserByChar(char name, string Value){

        //소문자는 자연증감, 대문자는 인위증감
        switch(name){

            //----------------------------Independent Value
           
            // 일반적인 Status_Value는 항상 음수기때문에, 정상적으로 Tick에 따라 차감됨.
            // Button에서 String을 할당할때만 0~50 사이의 값으로 지정되기때문에, 0~50 증가 51~99 감소 사용가능.

            //Hunger
            case 'u' :{
                if (float.TryParse(Value, out TryParseCheck))
                {
                    
                    float tempFloat = float.Parse(Value);

                    // By Status_Value
                    if (tempFloat < 0f) 
                    {
                        tempFloat *= hungerValue_Compensator;
                    }
                    
                    // By _StatusChangeButton
                    else if (tempFloat <= 50f) 
                    {
                        tempFloat *= u_Compensator;
                    }

                    // By _StatusChangeButton Over 50 negative 
                    else if (tempFloat > 50f)
                    {
                        tempFloat = (tempFloat - 50f) * -1f;
                        tempFloat *= u_Compensator;       
                    }

                    Value = tempFloat.ToString();
                    StatusIncrease(float.Parse(Value), ref status_hunger);

                    if (status_hunger >= hungerStackLevelTop ) { hungerStack++; }
                    else if (status_hunger <= hungerStackLevelBottom ) 
                    { 
                        hungerStack--;   
                        if (status_hunger == 0) { hungerStack--; hungerDeadStack++; };    
                    }
                } 
                break;
            }

            //Dirt
            case 'd' :{
                if (float.TryParse(Value, out TryParseCheck))
                {

                    float tempFloat = float.Parse(Value);

                    // By Status_Value
                    if (tempFloat < 0f)
                    {
                        tempFloat *= dirtValue_Compensator;
                    }

                    // By _StatusChangeButton
                    else if (tempFloat <= 50f)
                    {
                        tempFloat *= d_Compensator;
                    }

                    // By _StatusChangeButton Over 50 negative 
                    else if (tempFloat > 50f)
                    {
                        tempFloat = (tempFloat - 50f) * -1f;
                        tempFloat *= d_Compensator;
                    }

                    Value = tempFloat.ToString();
                    StatusIncrease(float.Parse(Value), ref status_dirt);

                    if (status_dirt >= dirtStackLevelTop) { dirtStack++; }
                    else if (status_dirt <= dirtStackLevelBottom)
                    {
                        dirtStack--;
                        if (status_dirt == 0) { dirtStack--; dirtDeadStack++; };
                    }
                }
                break;
            }

            //----------------------------Dependent Value

            //Happy
            case 'a' :{
                if (float.TryParse(Value, out TryParseCheck))
                {
                    float tempFloat = float.Parse(Value);

                    if(status_hunger < hungerToHappinessLevel * hungerToHappinessLevel_Compensator) 
                    { tempFloat += hungerToHappiness * hungerToHappiness_Compensator;}

                    if(status_dirt < dirtToHappinessLevel * dirtToHappinessLevel_Compensator)
                    { tempFloat += dirtToHappiness * dirtToHappiness_Compensator; }

                    tempFloat *= happyValue_Compensator;
                    Value = tempFloat.ToString();
                    StatusIncrease(float.Parse(Value), ref status_happiness);

                    if (status_happiness >= happyStackLevelTop) { happyStack++; }
                    else if (status_happiness <= happyStackLevelBottom)
                    {
                        happyStack--;
                        if (status_happiness == 0) { happyStack--; happyDeadStack++; };
                    }
                } 
                break;
            }

            //Health
            case 'e' :{
                if (float.TryParse(Value, out TryParseCheck))
                {
                    float tempFloat = float.Parse(Value); 

                    if (status_hunger < hungerToHealthLevel * hungerToHealthLevel_Compensator)
                    { tempFloat += hungerToHealth * hungerToHealth_Compensator; }

                    if (status_dirt < dirtToHealthLevel * dirtToHealthLevel_Compensator)
                    { tempFloat += dirtToHealth * dirtToHealth_Compensator; }

                    tempFloat *= healthValue_Compensator;
                    Value = tempFloat.ToString();

                    StatusIncrease(float.Parse(Value), ref status_health);

                    if (status_health >= healthStackLevelTop) { healthStack++; }
                    else if (status_health <= healthStackLevelBottom)
                    {
                        healthStack--;
                        if (status_health == 0) { healthStack--; healthDeadStack++; };
                    }
                }
                break;
            }

            //----------------------------By Incraser

            //Happy by Increaser
            case 'A' :{

                if(float.TryParse(Value, out TryParseCheck))
                {
                    float tempFloat = float.Parse(Value);
                    
                    if (float.Parse(Value) > 50)
                    {
                        tempFloat = ( float.Parse(Value) - 50 ) * -1f;      
                    }

                    tempFloat *= A_Compensator;

                    Value = tempFloat.ToString();
                    StatusIncrease(float.Parse(Value), ref status_happiness);
                }
                break;
            }
            
            //Health by Increaser
            case 'E' :{

                if (float.TryParse(Value, out TryParseCheck))
                {
                    float tempFloat = float.Parse(Value);

                    if (float.Parse(Value) > 50)
                    {
                        tempFloat = (float.Parse(Value) - 50) * -1f;    
                    }

                    tempFloat *= E_Compensator;

                    Value = tempFloat.ToString();
                    StatusIncrease(float.Parse(Value), ref status_health);
                }
                break;
            }

            //----------------------------HiddnStatus
            //Loneiness 
            case 'L' :{

                if (float.TryParse(Value, out TryParseCheck))
                {
                    float tempFloat = float.Parse(Value);

                    if (float.Parse(Value) > 50)
                    {
                        tempFloat = (float.Parse(Value) - 50) * -1f;
                    }

                    tempFloat *= loneiness_Compensator;

                    Value = tempFloat.ToString();
                    StatusIncrease(float.Parse(Value), ref status_loneiness);
                }
                break;
            }

            //goodness
            case 'g':
            {

                if (float.TryParse(Value, out TryParseCheck))
                {
                    float tempFloat = float.Parse(Value);

                    if (float.Parse(Value) > 50)
                    {
                        tempFloat = (float.Parse(Value) - 50) * -1f;
                    }

                    tempFloat *= goodness_Compensator;

                    Value = tempFloat.ToString();
                    StatusIncrease(float.Parse(Value), ref status_goodness);
                }
                break;
            }

            //badness
            case 'b':
            {

                if (float.TryParse(Value, out TryParseCheck))
                {
                    float tempFloat = float.Parse(Value);

                    if (float.Parse(Value) > 50)
                    {
                        tempFloat = (float.Parse(Value) - 50) * -1f;
                    }

                    tempFloat *= badness_Compensator;

                    Value = tempFloat.ToString();
                    StatusIncrease(float.Parse(Value), ref status_badness);
                }
                break;
            }

            //nightNday
            case 'n':
            {

                if (float.TryParse(Value, out TryParseCheck))
                {
                    float tempFloat = float.Parse(Value);

                    if (float.Parse(Value) > 50)
                    {
                        tempFloat = (float.Parse(Value) - 50) * -1f;
                    }

                    tempFloat *= nightNday_Compensator;

                    Value = tempFloat.ToString();
                    StatusIncrease(float.Parse(Value), ref status_nightNday);
                }
                break;
            }

            //psysical
            case 'p':
            {

                if (float.TryParse(Value, out TryParseCheck))
                {
                    float tempFloat = float.Parse(Value);

                    if (float.Parse(Value) > 50)
                    {
                        tempFloat = (float.Parse(Value) - 50) * -1f;
                    }

                    tempFloat *= psysical_Compensator;

                    Value = tempFloat.ToString();
                    StatusIncrease(float.Parse(Value), ref status_psysical);
                }
                break;
            }

            //intelligence
            case 'I':
            {

                if (float.TryParse(Value, out TryParseCheck))
                {
                    float tempFloat = float.Parse(Value);

                    if (float.Parse(Value) > 50)
                    {
                        tempFloat = (float.Parse(Value) - 50) * -1f;
                    }

                    tempFloat *= intelligence_Compensator;

                    Value = tempFloat.ToString();
                    StatusIncrease(float.Parse(Value), ref status_intelligence);
                }
                break;
            }
        }

        //변화된 Value들을 PlayerPrefs에 쑤셔넣기.
        StatusSynchronizerPerfsFromValue();
    }

    //스테이터스 변경 기본공식 / 보정값은 괄호 안쪽만 안건드리면 자유롭게 조정가능
    public float StatusIncrease( float Value, ref float Status ){

        Status += Value;

        // 스테이터스가 음수로 넘어가지 않도록
        if (Status < 0){ Status = 0f; }
        else if (Status > 100) { Status = 100f; }
        
        return Status;
    }

    //Start에 넣어서, 켰을때 이전 Status와 동기화하도록
    void StatusSynchronizerValueFromPerfs()
    {
        //------------------------playerperfsZone

        status_hunger = PlayerPrefs.GetFloat("status_hunger"); // 'u'
        status_dirt = PlayerPrefs.GetFloat("status_dirt"); // 'd'

        status_happiness = PlayerPrefs.GetFloat("status_happiness"); // 'a' 'A'
        status_health = PlayerPrefs.GetFloat("status_health"); // 'e' 'E'

        status_loneiness = PlayerPrefs.GetFloat("status_loneiness"); // 'L'
        status_goodness = PlayerPrefs.GetFloat("status_goodness"); // 'g'
        status_badness = PlayerPrefs.GetFloat("status_badness"); // 'b'
        status_nightNday = PlayerPrefs.GetFloat("status_nightNday"); // 'n'
        status_psysical = PlayerPrefs.GetFloat("status_psysical"); // 'p'
        status_intelligence = PlayerPrefs.GetFloat("status_intelligence"); // 'I'

        hungerStack = PlayerPrefs.GetInt("hungerStack");
        hungerDeadStack = PlayerPrefs.GetInt("hungerDeadStack");

        dirtStack = PlayerPrefs.GetInt("dirtStack");
        dirtDeadStack = PlayerPrefs.GetInt("dirtDeadStack");

        happyStack = PlayerPrefs.GetInt("happyStack");
        happyDeadStack = PlayerPrefs.GetInt("happyDeadStack");

        healthStack = PlayerPrefs.GetInt("healthStack");
        healthDeadStack = PlayerPrefs.GetInt("healthDeadStack");
    }

    //StatusIncreaseByChar에 넣어서, 스테이터스가 변화할때마다 Perfs에 저장하도록.
    void StatusSynchronizerPerfsFromValue(){

        PlayerPrefs.SetFloat("status_hunger", status_hunger); // 'u'
        PlayerPrefs.SetFloat("status_dirt", status_dirt); // 'd'

        PlayerPrefs.SetFloat("status_happiness", status_happiness); // 'a' 'A'
        PlayerPrefs.SetFloat("status_health", status_health); // 'e' 'E'

        PlayerPrefs.SetFloat("status_loneiness", status_loneiness); // 'L'
        PlayerPrefs.SetFloat("status_goodness", status_goodness); // 'g'
        PlayerPrefs.SetFloat("status_badness", status_badness); // 'b'
        PlayerPrefs.SetFloat("status_nightNday", status_nightNday); // 'n'
        PlayerPrefs.SetFloat("status_psysical", status_psysical); // 'p'
        PlayerPrefs.SetFloat("status_intelligence", status_intelligence); // 'I'

        PlayerPrefs.SetInt("hungerStack", hungerStack);
        PlayerPrefs.SetInt("hungerDeadStack", hungerDeadStack);
        
        PlayerPrefs.SetInt("dirtStack", dirtStack);
        PlayerPrefs.SetInt("dirtDeadStack", dirtDeadStack);

        PlayerPrefs.SetInt("happyStack", happyStack);
        PlayerPrefs.SetInt("happyDeadStack", happyDeadStack);

        PlayerPrefs.SetInt("healthStack", healthStack);
        PlayerPrefs.SetInt("healthDeadStack", healthDeadStack);
    }


    // CSV로 만들어서, name으로 Status동기화 하듯 동기화하는 수밖에 없는듯. 
    // Perf로 만들어야 하는걸 그냥 변수로 선언한 내 업보다 업보.

    // - Shop

    // 앞의 한글자로 어느 Charm인지 구분하고, 나머지 글자가 IndexNumber가 됨.
    public void _CompenSaterChanger(string charmIndexNumber){

        string charm_N_List = "Charm" + charmIndexNumber.Substring(0, 1);
        PlayerPrefs.SetInt(charm_N_List, int.Parse( charmIndexNumber.Substring(1, charmIndexNumber.Length - 1)) );

        // 변경하고, 동기화.
        CompensatorSynchronizer();
    }

    void CompensatorSynchronizer()
    {
        List<Dictionary<string, object>> Charms0List = CSVReader.Read("Charms/Charms0Status");
        List<Dictionary<string, object>> Charms1List = CSVReader.Read("Charms/Charms1Status");
        List<Dictionary<string, object>> Charms2List = CSVReader.Read("Charms/Charms2Status");

        // Hidden Status -------------------------

        charmSummation(Charms0List, Charms1List, Charms2List, "L", ref loneiness_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "g", ref goodness_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "b", ref badness_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "n", ref nightNday_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "p", ref psysical_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "I", ref intelligence_Compensator);

        charmSummation(Charms0List, Charms1List, Charms2List, "hV", ref hungerValue_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "hTA", ref hungerToHappiness_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "hTAL", ref hungerToHappinessLevel_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "hTE", ref hungerToHealth_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "hTEL", ref hungerToHealthLevel_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "u", ref u_Compensator);

        charmSummation(Charms0List, Charms1List, Charms2List, "dV", ref dirtValue_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "dTA", ref dirtToHappiness_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "dTAL", ref dirtToHappinessLevel_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "dTE", ref dirtToHealth_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "dTEL", ref dirtToHealthLevel_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "d", ref d_Compensator);

        charmSummation(Charms0List, Charms1List, Charms2List, "aV", ref happyValue_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "A", ref A_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "eV", ref healthValue_Compensator);
        charmSummation(Charms0List, Charms1List, Charms2List, "E", ref E_Compensator);

        // 장비와 악세사리를 탈착할 때, 이걸로 Compensator들을 업데이트함.
    }

    float charmSummation( List<Dictionary<string, object>> Charms0, List<Dictionary<string, object>> Charms1, List<Dictionary<string, object>> Charms2, string name , ref float Compensator){

        int C0 = PlayerPrefs.GetInt("Charm0");
        int C1 = PlayerPrefs.GetInt("Charm1");
        int C2 = PlayerPrefs.GetInt("Charm2");

        Compensator = 1 + float.Parse(Charms0[C0][name].ToString()) + float.Parse(Charms1[C1][name].ToString()) + float.Parse(Charms2[C2][name].ToString());

        return Compensator;
    }

    public GameObject[] Charm_0Index;
    public GameObject[] Charm_1Index;
    public GameObject[] Charm_2Index;


    public void _ShopOnoff(){

        for (int i = 0; i < Charm_0Index.Length; i++)
        { Charm_0Index[i].SetActive(false); }
        for (int i = 0; i < Charm_1Index.Length; i++)
        { Charm_1Index[i].SetActive(false);  }
        for (int i = 0; i < Charm_2Index.Length; i++)
        { Charm_2Index[i].SetActive(false);   }

        Charm_0Index[PlayerPrefs.GetInt("Charm0")].SetActive(true);
        Charm_1Index[PlayerPrefs.GetInt("Charm1")].SetActive(true);
        Charm_2Index[PlayerPrefs.GetInt("Charm2")].SetActive(true);

    }

    // - Shop 

    //Dictionary -  

    public GameObject[] dictionaryIndex;
    public Text[] dictionaryName;

    // 열고 닫을때 동기화해주면 될 듯.
    public void _DictionaryOnOff(){

        List<Dictionary<string, object>> data = CSVReader.Read("TempCaracter/SkeletonStatus");

        for (int i = 0; i < dictionaryIndex.Length; i++)
        {
            if(PlayerPrefs.HasKey("Creature"+i.ToString()))
            { 
                dictionaryIndex[i].SetActive(true);
                dictionaryName[i].text = data[i]["name"].ToString();
            }
            else
            { 
                dictionaryIndex[i].SetActive(false);
                dictionaryName[i].text = "???";
            }
        }
    }

    public GameObject dictionaryYesNo;
    public Text dictionaryYesNoName;
    public Text dictionaryYesNoDisc;
    public GameObject dictionaryYesNo_YesButton;
    public GameObject dictionaryNoCoin;
    public Text dictionaryNoCoinText;

    public void _DictionaryYesNoSet(int index)
    {

        List<Dictionary<string, object>> data = CSVReader.Read("TempCaracter/SkeletonStatus");
        dictionaryYesNoName.text = "선택 : " + data[index]["name"].ToString();
        dictionaryYesNoDisc.text = "설명 : " + data[index]["disc"].ToString();
        dictionaryYesNo_YesButton.name = index.ToString();
        dictionaryYesNo.SetActive(true);
    }

    public void _DictionaryEvolution()
    {
        if(PlayerPrefs.GetInt("Coin") >= 100)
        { 
            CreateCreature(int.Parse(dictionaryYesNo_YesButton.name));
            Debug.Log(dictionaryYesNo_YesButton.name + "번째 달팽이로 진화했습니다.");
    

            //코인 증가.
            _CoinIncrease(-100);
            // Skeleton이 교체된 이후엔, 스테이터스와 시간을 초기화함.
            statusResetter();
            // 모든 코루틴을 중단하고, 변화된 스테이터스에 따라 초기화.
            StatusIncreaseSetup();

            dictionaryYesNo.SetActive(false);
            onoffPanel[2].SetActive(false);
        }
        else 
        {
            dictionaryNoCoin.SetActive(true);
            dictionaryNoCoinText.text = PlayerPrefs.GetInt("Coin").ToString();
        }
    }

    public void _DictionaryYesNoExit()
    {
        dictionaryYesNo.SetActive(false);
    }

    public void _DictionaryNoCoinExit(){

        dictionaryNoCoin.SetActive(false);
        dictionaryYesNo.SetActive(false);

    }



    // - Dictionary


    public bool animationPalyChecker = false;

    public void _AnimationControll(string name)
    {
        if (!animationPalyChecker)
        {
            StartCoroutine(SpineAnimationCorutine(name));
            animationPalyChecker = true;
        }
    }

    IEnumerator SpineAnimationCorutine(string name)
    {
        TouchBlock.SetActive(true);

        string tempName = CreatureSkeleton.AnimationName;
        CreatureSkeleton.AnimationState.SetAnimation(0, name, true).TimeScale = 1f;
        
        yield return new WaitForSeconds(2f);

        CreatureSkeleton.AnimationState.SetAnimation(0, tempName, true).TimeScale = 1f;
        animationPalyChecker = false;

        TouchBlock.SetActive(false);
    }

    public GameObject[] Objects;

    public void _ObjectOnOff(int number){

        for (int i = 0; i < Objects.Length; i++)
        { Objects[i].SetActive(false); }

        StartCoroutine(ObjectsCorutine(number));

    }
    IEnumerator ObjectsCorutine(int indexNumber)
    {
        Objects[indexNumber].SetActive(true);
        Objects[indexNumber].GetComponent<SkeletonAnimation>().AnimationState.SetAnimation(0, "stand", true).TimeScale = 1f;
        yield return new WaitForSeconds(2f);
        Objects[indexNumber].SetActive(false);
    }

    // ----------------------------------------------------------------------- UI controller

    //For UI
    //public SkeletonGraphic skeletonGraphic;

    public Slider slider_hunger;
    public Slider slider_happiness;
    public Slider slider_health;
    public Slider slider_dirt;

    public GameObject[] dirts;
    
    public Text Text_Coin;
    public Text Text_Stamina;
    public Text Text_StaminaTime;

    public Text Text_ADTime;
    
    void Update()
    {
        slider_hunger.value = status_hunger;
        slider_happiness.value = status_happiness;
        slider_health.value = status_health;
        slider_dirt.value = status_dirt;
        try
        {   Text_Stamina.text = PlayerPrefs.GetFloat("Stamina").ToString();
            Text_Coin.text = PlayerPrefs.GetInt("Coin").ToString();
            Text_StaminaTime.text = "♥ 충전까지 앞으로 " + PlayerPrefs.GetInt("StaminaSettime").ToString()+"분";
        }

        catch{ }

        if (Input.GetKeyDown(KeyCode.Escape)){

            bool panelActiveChecker = false;

            for(int i = 0; i < onoffPanel.Length; i ++)
            { try { if (onoffPanel[i].activeSelf && !dictionaryYesNo.activeSelf ) { onoffPanel[i].SetActive(false); panelActiveChecker = true; } } catch { } }
            
            if (!panelActiveChecker && !AreYouSurePanel.activeSelf && !dictionaryYesNo.activeSelf) { onoffPanel[6].SetActive(true); }
            
            AreYouSurePanel.SetActive(false);
        }

        dirts[0].SetActive(false); dirts[1].SetActive(false); dirts[2].SetActive(false);

        if (status_dirt < 70f){
            dirts[0].SetActive(true);
            if(status_dirt < 40f){
                dirts[1].SetActive(true);
                if (status_dirt < 20f){
                    dirts[2].SetActive(true);
                }
            }
        }
        
        //AD Time 부분
        float LastTime = 0f;
        float OnTime = 0f;
        TimeToNumber(PlayerPrefs.GetString("ADTime"), ref LastTime);
        TimeToNumber(System.DateTime.Now.ToString("yyyy-MM-dd-HH-mm"), ref OnTime);

        if (OnTime - LastTime > 30) { Text_ADTime.text = "사용가능!"; }
        else 
        {
            Text_ADTime.text = (30 - (OnTime - LastTime)).ToString() + " 분!";
            if (30 - (OnTime - LastTime) == 0) { Text_ADTime.text = "잠시 기다려주세요"; }
        }
    }

    public void StaminaSettime(){

        int StaminasetTime = PlayerPrefs.GetInt("StaminaSettime");

        if ( StaminasetTime != 1 )
        { PlayerPrefs.SetInt("StaminaSettime", StaminasetTime - 1); }
        else 
        {
            float tempStamina = PlayerPrefs.GetFloat("Staina");

            _StaminaIncrease(30);

            PlayerPrefs.SetInt("StaminaSettime", 10);  
        }
    }

    public void _StaminaIncrease(int number){

        float tempStamina = PlayerPrefs.GetFloat("Stamina");

        if (number > 50) { number = (number - 50) * -1; }
        StatusIncrease(number, ref tempStamina);

        PlayerPrefs.SetFloat("Stamina", tempStamina);
    }

    public GameObject TextBox;
    public TextMesh text;

    public string[] textList = new string[15];

    public string TextShowPattern(ref string Text){

        //full	hunger	clean	dirt	happy   sad  hurt good	bad	psysical	intelligence	loney   free1 free2 free3


        int indexNumber = PlayerPrefs.GetInt("IndexNumber");

        List<Dictionary<string, object>> data = CSVReader.Read("TempCaracter/SkeletonTextScript");

        int indexCounter = 0;

        

        if (status_hunger > hungerStackLevelTop){ textList[indexCounter] = data[indexNumber]["full"].ToString(); indexCounter++; }
        if (status_hunger < hungerStackLevelBottom) { textList[indexCounter] = data[indexNumber]["hunger"].ToString(); indexCounter++; }
        if (status_dirt > dirtStackLevelTop) { textList[indexCounter] = data[indexNumber]["clean"].ToString(); indexCounter++; }
        if (status_dirt < dirtStackLevelBottom) { textList[indexCounter] = data[indexNumber]["dirt"].ToString(); indexCounter++; }
        if (status_happiness > happyStackLevelTop) { textList[indexCounter] = data[indexNumber]["happy"].ToString(); indexCounter++; }
        if (status_happiness < happyStackLevelBottom) { textList[indexCounter] = data[indexNumber]["sad"].ToString(); indexCounter++; }
        
        if (status_health < healthStackLevelBottom) { textList[indexCounter] = data[indexNumber]["hurt"].ToString(); indexCounter++; }

        if (status_goodness > 10) { textList[indexCounter] = data[indexNumber]["good"].ToString(); indexCounter++; }
        if (status_badness > 10) { textList[indexCounter] = data[indexNumber]["bad"].ToString(); indexCounter++; }
        if (status_psysical > 10) { textList[indexCounter] = data[indexNumber]["psycial"].ToString(); indexCounter++; }
        if (status_intelligence > 10) { textList[indexCounter] = data[indexNumber]["intelligence"].ToString(); indexCounter++; }
        if (status_loneiness > 10) { textList[indexCounter] = data[indexNumber]["loney"].ToString(); indexCounter++; }

        textList[indexCounter] = data[indexNumber]["free1"].ToString(); indexCounter++;
        textList[indexCounter] = data[indexNumber]["free2"].ToString(); indexCounter++;
        textList[indexCounter] = data[indexNumber]["free3"].ToString();

        int RandomNumber = UnityEngine.Random.Range(0,indexCounter);

        Text = textList[RandomNumber];

        return Text;
    }



    public void _ShowText()
    {
        string script = null;

        if(PlayerPrefs.GetInt("IndexNumber") == -1)
        {
            //-1에서 진화시키자.
            //DoEvolution();
            AreYouSurePanel.SetActive(true);
            AreYouSureText.text = "새로운 달팽이를 키워볼까요?";
        }
        else if(!TextBox.activeSelf)
        {

            TextShowPattern(ref script);
            text.text = script;

            StartCoroutine("ShowText"); 
        }
    }

    IEnumerator ShowText()
    {
        TextBox.SetActive(true);
        yield return new WaitForSeconds(3f);
        TextBox.SetActive(false);
    }

    public GameObject[] onoffPanel; 
    
    public void _PanelOnOff(int number)
    {
        if(number != 7)
        { 
            _PanelOff();
            onoffPanel[number].SetActive(true);
        }
        else 
        {
            //Credits가 자꾸 꺼져서, 일단 임시방편
            onoffPanel[number].SetActive(true);
        }
    }

    public void _PanelOff(){
        try{ 
            for (int i = 0; i < onoffPanel.Length; i++)
            { onoffPanel[i].SetActive(false); }
        }catch{ }
    }

    public void _Exit(){

        Application.Quit();
        
    }
}
