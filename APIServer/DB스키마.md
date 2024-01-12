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
게임 정보를 가지고 있는 테이블
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