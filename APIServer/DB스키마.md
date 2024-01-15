# account DB
  
## user_info 테이블
게임에서 생성 된 계정 정보들을 가지고 있는 테이블    
  
```sql
-- 테이블 생성 SQL - user_info
CREATE TABLE user_info
(
    `uid`                    INT            NOT NULL    AUTO_INCREMENT COMMENT '유저아이디', 
    `player_id`              BIGINT         NOT NULL    COMMENT '플레이어 아이디', 
    `nickname`               VARCHAR(50)    NOT NULL    COMMENT '닉네임', 
    `create_dt`              DATETIME       NOT NULL    COMMENT '생성 일시', 
    `recent_login_dt`        DATETIME       NULL        COMMENT '최근 로그인 일시', 
    `bestscore_ever`         INT            NOT NULL    COMMENT '최고점수 역대', 
    `bestscore_cur_season`   INT            NOT NULL    COMMENT '최고점수 현재 시즌', 
    `bestscore_prev_season`  INT            NOT NULL    COMMENT '최고점수 이전 시즌', 
     PRIMARY KEY (uid)
);

```     

## friend 테이블
친구 정보를 가지고 있는 테이블  
```sql
-- 테이블 생성 SQL - friend
CREATE TABLE friend
(
    `uid`         INT         NOT NULL    COMMENT '유저아이디', 
    `friend_uid`  INT         NOT NULL    COMMENT '친구 유저아이디', 
    `accept_yn`   TINYINT     NOT NULL  DEFAULT 0  COMMENT '승인 유무', 
    `create_dt`   DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (uid, friend_uid)
);
-- Foreign Key 설정 SQL - friend(uid) -> user_info(uid)
ALTER TABLE friend
    ADD CONSTRAINT FK_friend_uid_user_info_uid FOREIGN KEY (uid)
        REFERENCES user_info (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;

-- Foreign Key 설정 SQL - friend(friend_uid) -> user_info(uid)
ALTER TABLE friend
    ADD CONSTRAINT FK_friend_friend_uid_user_info_uid FOREIGN KEY (friend_uid)
        REFERENCES user_info (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;
```
<br>  
<br>

# game DB

## game 테이블
유저의 게임 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - game
CREATE TABLE game
(
    `uid`                    INT         NOT NULL    COMMENT '유저아이디', 
    `game_id`                INT         NOT NULL    COMMENT '게임 아이디', 
    `bestscore`              INT         NOT NULL    COMMENT '최고점수', 
    `create_dt`              DATETIME    NOT NULL    COMMENT '생성 일시', 
    `bestscore_cur_season`   INT         NOT NULL    COMMENT '최고점수 현재 시즌', 
    `bestscore_prev_season`  INT         NOT NULL    COMMENT '최고점수 이전 시즌', 
    `new_record_dt`          DATETIME    NULL        COMMENT '신 기록 일시', 
    `recent_play_dt`         DATETIME    NULL        COMMENT '최근 플레이 일시', 
     PRIMARY KEY (uid, game_id)
);

-- Foreign Key 설정 SQL - game(uid) -> user_info(uid)
ALTER TABLE game
    ADD CONSTRAINT FK_game_uid_user_info_uid FOREIGN KEY (uid)
        REFERENCES account_db.user_info (uid) ON DELETE RESTRICT ON UPDATE RESTRICT;
```

## char_info 테이블
유저의 캐릭터 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - char_info
CREATE TABLE char_info
(
    `uid`               INT         NOT NULL    COMMENT '유저아이디', 
    `char_id`           INT         NOT NULL    COMMENT '캐릭터 아이디', 
    `char_level`        INT         NOT NULL    DEFAULT 1 COMMENT '캐릭터 레벨', 
    `skin_id`           INT         NOT NULL    DEFAULT 0 COMMENT '스킨 아이디', 
    `costume1_id`       INT         NOT NULL    DEFAULT 0 COMMENT '코스튬1 아이디', 
    `costume2_id`       INT         NOT NULL    DEFAULT 0 COMMENT '코스튬2 아이디', 
    `costume3_id`       INT         NOT NULL    DEFAULT 0 COMMENT '코스튬3 아이디', 
    `create_dt`         DATETIME    NOT NULL    COMMENT '생성 일시', 
    `random_skill1_id`  INT         NOT NULL    DEFAULT 0 COMMENT '랜덤 스킬1 아이디', 
    `random_skill2_id`  INT         NOT NULL    DEFAULT 0 COMMENT '랜덤 스킬2 아이디', 
    `random_skill3_id`  INT         NOT NULL    DEFAULT 0 COMMENT '랜덤 스킬3 아이디', 
    `random_skill4_id`  INT         NOT NULL    DEFAULT 0 COMMENT '랜덤 스킬4 아이디', 
    `random_skill5_id`  INT         NOT NULL    DEFAULT 0 COMMENT '랜덤 스킬5 아이디', 
    `random_skill6_id`  INT         NOT NULL    DEFAULT 0 COMMENT '랜덤 스킬6 아이디', 
     PRIMARY KEY (uid, char_id)
);

    
```

## costume_info 테이블
유저의 코스튬 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - costume_info
CREATE TABLE costume_info
(
	`uid`            INT         NOT NULL    COMMENT '유저아이디', 
    `costume_id`     INT         NOT NULL    COMMENT '코스튬 아이디', 
    `costume_level`  INT         NOT NULL    COMMENT '코스튬 레벨', 
    `create_dt`      DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (uid, costume_id)
);

```

## skin_info 테이블
유저의 스킨 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - skin_info
CREATE TABLE skin_info
(
	`uid`            INT         NOT NULL    COMMENT '유저아이디', 
    `skin_id`        INT         NOT NULL    COMMENT '코스튬 아이디', 
    `create_dt`      DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (uid, skin_id)
);
```

# master DB
## version 테이블
앱버전과 데이터 버전을 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - version
CREATE TABLE version
(
    `app_version`            INT         NOT NULL    COMMENT '앱 버전', 
    `master_data_version`    INT         NOT NULL    COMMENT '마스터 데이터 버전', 
);
```

## game_info 테이블
게임 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - game_info
CREATE TABLE game_info
(
    `game_id`                INT         NOT NULL    COMMENT '게임아이디', 
    `game_name`              INT         NOT NULL    COMMENT '게임 이름', 
);
```

## master_char 테이블
캐릭터 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - master_char
CREATE TABLE master_char
(
    `char_id`           INT         NOT NULL    COMMENT '캐릭터 아이디',
    `char_name`			VARCHAR(30)	NOT NULL 	COMMENT '캐릭터 이름',
    `char_grade`	 	VARCHAR(10) NOT NULL    COMMENT '캐릭터 등급',
    `stat_run`          INT         NOT NULL    COMMENT '달리기 스탯', 
    `stat_power`        INT         NOT NULL    COMMENT '힘 스탯', 
    `stat_jump`         INT         NOT NULL    COMMENT '점프 스탯', 
    `original_skill1_id` INT         NOT NULL    COMMENT '고유 스킬1 아이디',
    `original_skill2_id` INT         NOT NULL    COMMENT '고유 스킬2 아이디',
    `create_dt`         DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (char_id)
);
```

## master_skill 테이블
스킬 정보를 가지고 있는 테이블
```sql
CREATE TABLE master_skill
(
    `skill_id`          INT         NOT NULL    COMMENT '스킬아이디',
    `act_prob_percent`  INT         NOT NULL    COMMENT '발동 확률 퍼센트',
    `create_dt`         DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (skill_id)
);
```

## master_costume 테이블
코스튬 정보를 가지고 있는 테이블
```sql
-- 테이블 생성 SQL - master_costume
CREATE TABLE master_costume
(
    `costume_id`        INT         NOT NULL    COMMENT '코스튬 아이디', 
    `costume_name`      INT         NOT NULL    COMMENT '코스튬 이름', 
    `costume_type`      INT         NOT NULL    COMMENT '코스튬 타입',
    `set_id`			INT         NOT NULL    COMMENT '세트 아이디',
    `create_dt`         DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (costume_id)
);

```

## master_costume_set 테이블
코스튬 세트 정보를 가지고 있는 테이블
```sql
CREATE TABLE master_costume_set
(
    `set_id`            	INT         NOT NULL    COMMENT '세트 아이디', 
    `char_id`				INT         NOT NULL 	COMMENT '캐릭터 아이디',
	`set_name`				VARCHAR(30) NOT NULL 	COMMENT '세트 이름',
	`costume_bonus_percent` INT         NOT NULL    COMMENT '보너스 점수 퍼센트',
    `char_bonus_percent`	INT         NOT NULL    COMMENT '캐릭터 보너스 점수 퍼센트',
    `costume1_id`       	INT         NOT NULL    COMMENT '코스튬1 아이디',
    `costume2_id`       	INT         NOT NULL    COMMENT '코스튬2 아이디',
    `costume3_id`       	INT         NOT NULL    COMMENT '코스튬3 아이디',
    `create_dt`         	DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (set_id)
);
```

## master_skin 테이블
스킨 정보를 가지고 있는 테이블
```sql
-- 테이블 Comment 설정 SQL - master_skin
CREATE TABLE master_skin
(
    `skin_id`           INT         NOT NULL    COMMENT '스킨 아이디',
    `skin_name`         VARCHAR(30) NOT NULL    COMMENT '스킨 이름',
    `char_id`    	    INT         NOT NULL    COMMENT '캐릭터 아이디',
    `skin_bonus_percent`INT         NOT NULL    COMMENT '보너스 점수 퍼센트',
    `create_dt`         DATETIME    NOT NULL    COMMENT '생성 일시', 
     PRIMARY KEY (skin_id)
);
```