# Dalpangyeekiwoogi



![Icon 1](readmeImage.jpg)



## About The Project

다마고치에서 영감을 받아 제작한 키우기 게임



## Built With

- [![BuildWith](https://img.shields.io/badge/Unity-2020.3.19f1-green)](https://unity3d.com/kr/unity/whats-new/2020.3.19)



## Feature

체력 감소, 죽음, 진화, 밥주기, 놀아주기, 행동력 리필 등 기본 기능 구현됨




## Built With

UnityVersion : 2020.3.19f1

### Flow Chart

```mermaid
graph LR
	start
	InitClass
	RegistActions
	hasPlayerprefs{hasPlayerprefs}
	SetNewData
	LoadData
	calculateSleepedTickAction
	TickAction 
    isDead{isDead}
    canEvolve{canEveolve}
    createNowCreature
    
    start --> InitClass
    InitClass --> RegistActions
    InitClass --> hasPlayerprefs
    hasPlayerprefs --> SetNewData
    hasPlayerprefs --> LoadData
    RegistActions --> calculateSleepedTickAction
    SetNewData --> calculateSleepedTickAction
    LoadData --> calculateSleepedTickAction
    calculateSleepedTickAction --> TickAction
    TickAction --> isDead
    TickAction --> canEvolve
    isDead --> createNowCreature
    canEvolve --> createNowCreature
    createNowCreature --> TickAction
```





## Class Structure

```mermaid
classDiagram
	class ActionManager{
		list action
		RegistAction()
	}
	
	class ScriptableObjects{
		data datas
	}
	class DataManager{
		data Status
		data Player
		data CreatureList
	}
	
	class CreatureManager{
		spineAnimation creature
		animationState state
	}
	class UIManager{
		list btns
		list pnls 
	}
	
	ActionManager <-- DataManager : Regist Action
	ActionManager <-- CreatureManager : Regist Action
	ActionManager <-- UIManager : Regist&Execute Action
	DataManager <.. CreatureManager : CreautreList.Count
	ScriptableObjects <.. DataManager
```






## Update

V.0.0.1 (2020.03 ~ 04) 최초구현  
V.0.0.2 (2022.07 ~ 08) [리팩토링](https://baeknothing.tistory.com/66?category=1060113)



## Todo 

- [x] 리팩토링  
- [x] 리팩토링 정리  
- [ ] 유닛 테스트  
