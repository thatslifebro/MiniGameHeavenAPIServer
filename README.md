# 미니게임천국 API 서버


# TODO-LIST

완료한 작업 : ✅

- **하이브 서버 기능**
 
| 기능                                         | 완료 여부 |
| -------------------------------------------- | --------- |
| 하이브 계정생성   						| ✅        |
| [하이브 로그인]							| ✅        |
| [하이브 토큰 검증]								 | ✅        |

- **계정 기능**

| 기능                                         | 완료 여부 |
| -------------------------------------------- | --------- |
| [로그인]						              | ✅        |
| [로그아웃]								       | ✅        |
| [게임 데이터 로드]	                		 | ✅        |

- **친구 기능**

| 기능                                            | 완료 여부 |
| ----------------------------------------------- | --------- |
| [친구 목록 조회]								  | ✅        |
| [친구 요청]								  | ✅        |
| [친구 받은 요청 조회]								  | ✅        |
| [친구 보낸 요청 조회]								  | ✅        |
| [친구 삭제]								  | ✅        |
| [친구 요청 취소]								  | ✅        |


- **게임 기능**
 
| 기능                                            | 완료 여부 |
| ----------------------------------------------- | --------- |
| [보유 게임 조회]								  | ✅        |
| [게임 잠금 해제]								  | ✅        |
| [게임별 정보 조회]					          | ✅        |
| [게임 결과 저장]								  | ✅        |
| [전체 랭킹 조회]								  | ⬜        |


- **출석 기능**

| 기능                              | 완료 여부 |
| --------------------------------- | --------- |
| [출석 정보 조회]					| ⬜        |
| [출석 체크]						| ⬜        |

- **우편 기능**

| 기능                                            | 완료 여부 |
| ----------------------------------------------- | --------- |
| [우편함 조회]										| ⬜        |
| [우편 아이템 수령]	(일반 수령 및 가챠 수령 구현)		| ⬜        |
| 우편 삭제                                       | ⬜        |


- **캐릭터 기능**

| 기능                                            | 완료 여부 |
| ----------------------------------------------- | --------- |
| [캐릭터 조회]								  | ⬜        |
| [캐릭터 상세 조회]						      | ⬜        |
| [코스튬 조회]								  | ⬜        |
| [캐릭터 코스튬 변경]								  | ⬜        |
| [캐릭터 스킨 변경]						      | ⬜        |

- **배틀 기능**

| 기능                                            | 완료 여부 |
| ----------------------------------------------- | --------- |
| [메달 배틀 정보 조회]					  | ⬜        |
| [메달 배틀 리그 랭킹]								  | ⬜        |
| [메달 배틀 전체 랭킹]								  | ⬜        |
| [클랜 배틀 정보 조회]							  | ⬜        |
| [클랜 배틀 랭킹]								  | ⬜        |
| [팀 배틀 정보 조회]					  | ⬜        |


- **클랜 기능**

| 기능                                            | 완료 여부 |
| ----------------------------------------------- | --------- |
| [클랜 조회]								  | ⬜        |
| [클랜원 목록 조회]		                 | ⬜        |
| [클랜원 정보 조회]		                 | ⬜        |


- **이벤트 및 공지사항 기능**
 
| 기능                                            | 완료 여부 |
| ----------------------------------------------- | --------- |
| [이벤트 조회]								 | ⬜        |
| [공지사항 조회]								 | ⬜        |


- **퀘스트 및 도전과제 기능**

| 기능                                            | 완료 여부 |
| ----------------------------------------------- | --------- |
| [퀘스트 조회]								 | ⬜        |
| [도전과제 조회]								 | ⬜        |

- **천국 은행 기능**

| 기능                                            | 완료 여부 |
| ----------------------------------------------- | --------- |
| [썬칩 금메달 교환]								 | ⬜        |


- **사용자 기능**

| 기능                                         | 완료 여부 |
| -------------------------------------------- | --------- |
| [내 정보 조회]		                 | ⬜        |
| [대표 캐릭터 변경]						 | ⬜        |
| [칭호 변경]					        	 | ⬜        |
| [사용자 조회]		                 | ⬜        |


- **상점 기능**

| 기능                                            | 완료 여부 |
| ----------------------------------------------- | --------- |
| [보석 패키지 구매]								  | ⬜        |
| [메달 패키지 구매]								  | ⬜        |
| [현금 패키지 구매]								  | ⬜        |
| [푸드 변환]								  | ⬜        |
| [푸드 기어 변환]								  | ⬜        |



## 하이브 로그인
(LoginHive)

**컨텐츠 설명**
- 하이브에 로그인 하여 고유번호와 토큰을 받습니다.

**로직**
1. 클라이언트가 이메일과 비밀번호를 하이브 서버에 전달한다.
1. 클라이언트의 고유번호와 생성된 토큰을 응답한다. 


클라이언트 → 서버 전송 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 이메일               | 로그인 이메일 |
| 비밀번호             | 로그인 비밀번호 |


#### 요청 및 응답 예시

- 요청 예시

```
POST http://localhost:11502/loginhive
Content-Type: application/json

{
    "Email" : "example@test.com",
    "Password" : "Aslj3kldiu!",
}
```

- 응답 예시

```
{
    "result": 0,
    "playerId": 7,
    "hiveToken": "efaee4517404318a8d14f6053767ff74dcf9aw30910b9116dafd3fa4ce408a45"
}
```

---


## 하이브 토큰 검증

**컨텐츠 설명**
- 고유번호와 토큰이 유효한지 확인합니다.

**로직**
1. 클라이언트가 고유번호와 토큰을 전달한다.
1. 고유번호를 토큰화 하여 가지고 있는 토큰과 일치하는지 응답한다.


클라이언트 → 서버 전송 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 고유번호               | 로그인시 받은 playerId |
| 토큰                  | 로그인시 받은 token |


#### 요청 및 응답 예시

- 요청 예시

```
POST http://localhost:11502/verifytoken
Content-Type: application/json

{
    "PlayerId" : 58347,
    "Token" : "efaee4517404318a8d14f6053767ff74dcf9aw30910b9116dafd3fa4ce408a45!"
}
```

- 응답 예시

```
{
    "result": 0
}
```

---

## 로그인

**컨텐츠 설명**
- 하이브에서 얻은 고유번호와 토큰을 가지고 게임서버에 로그인 합니다.

**로직**
1. 고유번호와 토큰, 앱 버전, 마스터 데이터 버전을 게임 서버에 전달.
1. [미들웨어] 앱 버전과 마스터 데이터 버전 검증
1. 게임 서버가 고유번호와 토큰을 하이브 서버에 검증.
1. 게임 서버가 새로운 토큰을 생성하여 클라이언트에 전달.
1. 게임 서버가 Redis에 토큰 저장
1. 게임 데이터 로드 (버전 데이터 + 게임 데이터 정의 후 추가 예정)
1. 최근 로그인 시간 갱신

클라이언트 → 서버 전송 데이터
- Header 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 앱 버전 정보           |     |
| 게임 데이터 정보      |      |

- Body 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 고유번호               | Hive 로그인시 받은 playerId |
| 토큰                  | Hive 로그인시 받은 token |


#### 요청 및 응답 예시

- 요청 예시

```
POST http://localhost:11500/login

AppVersion : "0.1",
MasterDataVersion : "0.1"

Content-Type: application/json
{
    "PlayerId" : 58347,
    "Token" : "efaee4517404318a8d14f6053767ff74dcf9aw30910b9116dafd3fa4ce408a45!",
}
```

- 응답 예시
```
{
    "result": 0,
    "token": "ijqdlgbfcbffwhw79jck5wbg4",
    "uid": 1,
}
```

게임 데이터 정의 완료 후 추가 예정

---

## 로그아웃

**컨텐츠 설명**
- redis에 저장된 토큰을 지웁니다.

**로직**

1. 유저아이디, 토큰, 앱 버전, 마스터 데이터 버전을 게임 서버에 전달.
1. [미들웨어] 앱 버전과 마스터 데이터 버전 검증
1. [미들웨어] 토큰 검증
1. 게임 서버가 Redis에서 토큰 삭제

클라이언트 → 서버 전송 데이터

- Header 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 유저아이디               | 게임서버의 uid |
| 토큰                | 레디스에 저장되어 있는 토큰 |
| 앱 버전 정보        |  |
| 게임 데이터 정보     |  |


#### 요청 및 응답 예시

- 요청 예시

```
DELETE http://localhost:11500/logout

AppVersion : "0.1",
MasterDataVersion : "0.1"
Uid : 1
Token : "c9v3arfa83vaugm0rxb7txm0c!"

Content-Type: application/json
{
}
```

- 응답 예시


```
{
    "result": 0
}
```

---

## 게임 데이터 로드

**컨텐츠 설명**
- 유저의 데이터를 모두 가져옵니다.

**로직**
1. 유저아이디, 토큰, 앱 버전, 마스터 데이터 버전을 게임 서버에 전달.
1. [미들웨어] 앱 버전과 마스터 데이터 버전 검증
1. [미들웨어] 토큰 검증
1. 유저의 정보를 전달

    - 점수 정보
    - 재화 정보
    - 친구 정보
    - 게임 정보
    - 캐릭터, 스킨, 코스튬, 푸드 정보
    - 우편 정보
    - 출석 정보

클라이언트 → 서버 전송 데이터
- Header 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 유저아이디               | 게임서버의 uid |
| 토큰                | 레디스에 저장되어 있는 토큰 |
| 앱 버전 정보        | 헤더에 포함 |
| 게임 데이터 정보     | 헤최고더에 포함 |

#### 요청 및 응답 예시

- 요청 예시

```
GET http://localhost:11500/DataLoad

AppVersion : "0.1",
MasterDataVersion : "0.1"
Uid : 1
Token : "c9v3arfa83vaugm0rxb7txm0c!"

Content-Type: application/json
{
}
```

- 응답 예시

```
{
    "userData": {
        "userInfo": {
            "uid": 6,
            "player_id": "18",
            "nickname": "ksy",
            "create_dt": "2024-01-17T14:02:09",
            "recent_login_dt": "2024-01-17T16:21:26",
            "bestscore_ever": 0,
            "bestscore_cur_season": 0,
            "bestscore_prev_season": 0,
            "star_point": 0
        },
        "moneyInfo": {
            "uid": 6,
            "jewelry": 0,
            "gold_medal": 0,
            "sunchip": 0,
            "cash": 0
        },
        "friendList": [],
        "gameList": [
            {
                "game_key": 1,
                "bestscore": 0,
                "create_dt": "2024-01-17T14:02:08",
                "new_record_dt": "0001-01-01T00:00:00",
                "recent_play_dt": "0001-01-01T00:00:00",
                "bestscore_cur_season": 0,
                "bestscore_prev_season": 0
            }
        ],
        "charList": [
            {
                "charInfo": {
                    "uid": 6,
                    "char_key": 1,
                    "char_level": 1,
                    "skin_key": 0,
                    "costume_json": null
                },
                "randomSkills": []
            }
        ],
        "skinList": [],
        "costumeList": [],
        "foodList": [],
        "mailList": [],
        "attendanceInfo": {
            "uid": 6,
            "attendance_count": 0,
            "recent_attendance_dt": "0001-01-01T00:00:00"
        }
    },
    "result": 0
}
```

---


## 친구 목록 조회

**컨텐츠 설명**
- 자신의 친구 목록을 조회합니다.

**로직**
1. 유저아이디, 토큰, 앱 버전, 마스터 데이터 버전, 정렬조건을 게임 서버에 전달.
1. [미들웨어] 앱 버전과 마스터 데이터 버전 검증
1. [미들웨어] 토큰 검증
1. 유저아이디와 정렬 조건을 통해 친구 목록 조회 및 전달
1. uid, 닉네임, 정렬조건의 점수를 전달.
- 정렬 조건
  
  bestscore_ever - 역대 최고 기록 (기본값)

  bestscore_cur_season - 현재 시즌 기록

  bestscore_prev_season - 이전 시즌 기록

클라이언트 → 서버 전송 데이터
- Header 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 유저아이디               | 게임서버의 uid |
| 토큰                | 레디스에 저장되어 있는 토큰 |
| 앱 버전 정보        | 헤더에 포함 |
| 게임 데이터 정보     | 헤최고더에 포함 |

- Query String 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 정렬조건 | 정렬조건 (역대 최고 기록, 현재 시즌 기록, 이전 시즌 기록) - 쿼리스트링 |


#### 요청 및 응답 예시

- 요청 예시

```
GET http://localhost:11500/friendlist?orderby=bestscore_ever

AppVersion : "0.1",
MasterDataVersion : "0.1"
Uid : 1
Token : "c9v3arfa83vaugm0rxb7txm0c!"

Content-Type: application/json
{
}
```

- 응답 예시


```
{
    "friendList": [
        {
            "uid": 1,
            "nickName": "ksy"
            "bestScore":4000000
        },
        {
            "uid": 3,
            "nickName": "asfev"
            "bestScore":3000000
        },
        {
            "uid": 4,
            "nickName": "sadasfev"
            "bestScore":2000000
        }
    ],
    "result": 0
}
```

---

## 친구 요청
**컨텐츠 설명**
- 유저 한명에게 친구 요청을 보냅니다.

**로직**
1. 유저아이디, 토큰, 앱 버전, 마스터 데이터 버전, 친구 아이디를 게임 서버에 전달.
1. [미들웨어] 앱 버전과 마스터 데이터 버전 검증
1. [미들웨어] 토큰 검증
1. 해당 유저의 아이디가 없는지 확인
1. 이미 친구신청을 보냈는지 확인
1. 상대방이 친구신청을 보낸 상태라면 서로 친구로 등록
1. 아니라면 친구신청을 보냄

클라이언트 → 서버 전송 데이터

- Header 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 유저아이디               | 게임서버의 uid |
| 토큰                | 레디스에 저장되어 있는 토큰 |
| 앱 버전 정보        | 헤더에 포함 |
| 게임 데이터 정보     | 헤더에 포함 |

- Body 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 친구 아이디 | 친구로 등록할 유저의 아이디 |



#### 요청 및 응답 예시

- 요청 예시

```
POST http://localhost:11500/friendadd

AppVersion : "0.1",
MasterDataVersion : "0.1"
Uid : 1
Token : "c9v3arfa83vaugm0rxb7txm0c!"

Content-Type: application/json

{
    "friendId" : 2
}
```

- 응답 예시


```
{
    "result": 0
}
```

---

## 친구 받은 요청 조회
**컨텐츠 설명**
- 자신이 받은 친구 요청을 조회합니다.

**로직**
1. 유저아이디, 토큰, 앱 버전, 마스터 데이터 버전을 게임 서버에 전달.
1. [미들웨어] 앱 버전과 마스터 데이터 버전 검증
1. [미들웨어] 토큰 검증
1. 유저아이디를 통해 받은 친구 요청 조회 및 전달


클라이언트 → 서버 전송 데이터

- Header 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 유저아이디               | 게임서버의 uid |
| 토큰                | 레디스에 저장되어 있는 토큰 |
| 앱 버전 정보        | 헤더에 포함 |
| 게임 데이터 정보     | 헤더에 포함 |



#### 요청 및 응답 예시

- 요청 예시

```
GET http://localhost:11500/friendreceivedreqlist

AppVersion : "0.1",
MasterDataVersion : "0.1"
Uid : 1
Token : "c9v3arfa83vaugm0rxb7txm0c!"

Content-Type: application/json

{
}
```

- 응답 예시


```
{
    "friendRequestList": [
        {
            "uid": 2,
            "nickName": "adslk"
        }
    ],
    "result": 0
}
```

---

## 친구 보낸 요청 조회
**컨텐츠 설명**
- 자신이 보낸 친구 요청을 조회합니다.

**로직**
1. 유저아이디, 토큰, 앱 버전, 마스터 데이터 버전을 게임 서버에 전달.
1. [미들웨어] 앱 버전과 마스터 데이터 버전 검증
1. [미들웨어] 토큰 검증
1. 유저아이디를 통해 보낸 친구 요청 조회 및 전달


클라이언트 → 서버 전송 데이터

- Header 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 유저아이디               | 게임서버의 uid |
| 토큰                | 레디스에 저장되어 있는 토큰 |
| 앱 버전 정보        | 헤더에 포함 |
| 게임 데이터 정보     | 헤더에 포함 |



#### 요청 및 응답 예시

- 요청 예시

```
GET http://localhost:11500/friendsentreqlist

AppVersion : "0.1",
MasterDataVersion : "0.1"
Uid : 1
Token : "c9v3arfa83vaugm0rxb7txm0c!"

Content-Type: application/json

{
}
```

- 응답 예시


```
{
    "friendRequestList": [
        {
            "uid": 2,
            "nickName": "adslk"
        }
    ],
    "result": 0
}
```

---

## 친구 삭제
**컨텐츠 설명**
- 한명의 친구를 삭제합니다.

**로직**
1. 유저아이디, 토큰, 앱 버전, 마스터 데이터 버전, 친구 아이디를 게임 서버에 전달.
1. [미들웨어] 앱 버전과 마스터 데이터 버전 검증
1. [미들웨어] 토큰 검증
1. 해당 아이디가 친구인지 확인
1. 양쪽 모두 친구 목록에서 삭제


클라이언트 → 서버 전송 데이터

- Header 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 유저아이디               | 게임서버의 uid |
| 토큰                | 레디스에 저장되어 있는 토큰 |
| 앱 버전 정보        | 헤더에 포함 |
| 게임 데이터 정보     | 헤더에 포함 |

- Body 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 친구 아이디 | 친구로 등록할 유저의 아이디 |


#### 요청 및 응답 예시

- 요청 예시

```
DELETE http://localhost:11500/friendsentreqlist

AppVersion : "0.1",
MasterDataVersion : "0.1"
Uid : 1
Token : "c9v3arfa83vaugm0rxb7txm0c!"

Content-Type: application/json

{
    "friendId" : 2
}
```

- 응답 예시


```
{
    "result": 0
}
```

---

## 친구 요청 취소
**컨텐츠 설명**
- 내가 보낸 친구 요청을 취소합니다.

**로직**
1. 유저아이디, 토큰, 앱 버전, 마스터 데이터 버전, 친구 아이디를 게임 서버에 전달.
1. [미들웨어] 앱 버전과 마스터 데이터 버전 검증
1. [미들웨어] 토큰 검증
1. 해당 친구 요청이 있는지 확인
1. 해당 친구 요청 삭제


클라이언트 → 서버 전송 데이터

- Header 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 유저아이디               | 게임서버의 uid |
| 토큰                | 레디스에 저장되어 있는 토큰 |
| 앱 버전 정보        | 헤더에 포함 |
| 게임 데이터 정보     | 헤더에 포함 |

- Body 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 친구 아이디 | 친구로 등록할 유저의 아이디 |


#### 요청 및 응답 예시

- 요청 예시

```
DELETE http://localhost:11500/friendcancelreq

AppVersion : "0.1",
MasterDataVersion : "0.1"
Uid : 1
Token : "c9v3arfa83vaugm0rxb7txm0c!"

Content-Type: application/json

{
    "friendId" : 2
}
```

- 응답 예시


```
{
    "result": 0
}
```

---

## 보유 게임 조회
**컨텐츠 설명**
- 내가 보유한 게임을 조회합니다.

**로직**
1. 유저아이디, 토큰, 앱 버전, 마스터 데이터 버전을 게임 서버에 전달.
1. [미들웨어] 앱 버전과 마스터 데이터 버전 검증
1. [미들웨어] 토큰 검증
1. game 테이블에 내 uid를 가진 로우를 조회하여 전달합니다.


클라이언트 → 서버 전송 데이터
- Header 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 유저아이디               | 게임서버의 uid |
| 토큰                | 레디스에 저장되어 있는 토큰 |
| 앱 버전 정보        | 헤더에 포함 |
| 게임 데이터 정보     | 헤더에 포함 |


#### 요청 및 응답 예시

- 요청 예시

```
GET http://localhost:11500/GameList

AppVersion : "0.1",
MasterDataVersion : "0.1"
Uid : 1
Token : "c9v3arfa83vaugm0rxb7txm0c!"

Content-Type: application/json

{
}
```

- 응답 예시


```
{
    "gameList": [
        {
            "game_key": 1,
            "game_name": "뚫어뚫어",
            "bestscore": 0,
            "create_dt": "01/12/2024 15:17:00"
        },
        {
            "game_key": 12,
            "game_name": "놓아놓아",
            "bestscore": 1000,
            "create_dt": "01/12/2024 15:50:35"
        }
    ],
    "result": 0
}
```

---

## 게임 잠금 해제
**컨텐츠 설명**
- 게임을 잠금 해제 합니다.

**로직**
1. 유저아이디, 토큰, 앱 버전, 마스터 데이터 버전, 게임아이디를 게임 서버에 전달.
1. [미들웨어] 앱 버전과 마스터 데이터 버전 검증
1. [미들웨어] 토큰 검증
1. game 테이블에 해당 uid 와 game_id로 로우를 생성합니다.


클라이언트 → 서버 전송 데이터
- Header 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 유저아이디               | 게임서버의 uid |
| 토큰                | 레디스에 저장되어 있는 토큰 |
| 앱 버전 정보        | 헤더에 포함 |
| 게임 데이터 정보     | 헤더에 포함 |

- Body 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 게임 키             | 게임의 고유 키 |

#### 요청 및 응답 예시

- 요청 예시

```
POST http://localhost:11500/GameUnlock

AppVersion : "0.1",
MasterDataVersion : "0.1"
Uid : 1
Token : "c9v3arfa83vaugm0rxb7txm0c!"

Content-Type: application/json

{
    "gameKey": 4
}
```

- 응답 예시


```
{
    "result": 0
}
```

---
## 게임별 정보 조회
**컨텐츠 설명**
- 게임 하나의 정보를 상세 조회합니다.

**로직**
1. 유저아이디, 토큰, 앱 버전, 마스터 데이터 버전, 게임아이디를 게임 서버에 전달.
1. [미들웨어] 앱 버전과 마스터 데이터 버전 검증
1. [미들웨어] 토큰 검증
1. 게임의 정보 (아이템 보유 현황, 플레이 캐릭터(코스튬,스킨)정보, 최고점수, 시즌 최고점수 등)을 조회합니다.
- 현재 아이템 및 캐릭터 정보가 없어 점수만 조회가능.

클라이언트 → 서버 전송 데이터
- Header 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 유저아이디               | 게임서버의 uid |
| 토큰                | 레디스에 저장되어 있는 토큰 |
| 앱 버전 정보        | 헤더에 포함 |
| 게임 데이터 정보     | 헤더에 포함 |

- Body 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 게임 키             | 게임의 고유 키 |

#### 요청 및 응답 예시

- 요청 예시

```
POST http://localhost:11500/GameUnlock

AppVersion : "0.1",
MasterDataVersion : "0.1"
Uid : 1
Token : "c9v3arfa83vaugm0rxb7txm0c!"

Content-Type: application/json

{
    "gameKey": 4
}
```

- 응답 예시


```
{
    "gameInfo": {
        "game_key": 12,
        "game_name": "놓아놓아",
        "bestscore": 1000,
        "create_dt": "01/12/2024 15:50:35",
        "new_record_dt": "01/12/2024 17:00:17",
        "recent_play_dt": "01/12/2024 17:00:17",
        "bestscore_cur_season": 1000,
        "bestscore_prev_season": 0
    },
    "result": 0
}
```

---

## 게임 결과 저장
**컨텐츠 설명**
- 미완성
- 게임 결과를 저장합니다.

**로직**
1. 유저아이디, 토큰, 앱 버전, 마스터 데이터 버전, 게임아이디 및 결과정보를 게임 서버에 전달.
1. [미들웨어] 앱 버전과 마스터 데이터 버전 검증
1. [미들웨어] 토큰 검증
1. 사용한 아이템 현황, 획득 보상, 점수 등을 업데이트 하고 로그에 기록합니다.
- 현재 아이템과 보상 정보가 없어 점수만 저장 가능

클라이언트 → 서버 전송 데이터

| 종류                  | 설명                             |
| --------------------- | -------------------------------- |
| 유저아이디               | 게임서버의 uid |
| 토큰                | 레디스에 저장되어 있는 토큰 |
| 앱 버전 정보        | 헤더에 포함 |
| 게임 데이터 정보     | 헤더에 포함 |
| 게임결과 | 사용아이템, 보상, 점수 등 |


#### 요청 및 응답 예시

- 요청 예시

```
POST http://localhost:11500/GameUnlock
Content-Type: application/json

{
    "uid" : 1,
    "Token" : "c9v3arfa83vaugm0rxb7txm0c!",
    "AppVersion" : "0.1",
    "MasterDataVersion" : "0.1",
    // 게임결과는 추후 정의
}
```

- 응답 예시


```
{
    "result": 0
}
```

---