# 유저의 로그인
```mermaid
sequenceDiagram
	actor User
	participant Game Server
	participant Fake Hive Server
	participant DB

	User->> Fake Hive Server: 로그인 요청
	Fake Hive Server -->> User : 고유번호와 토큰 전달

	User ->> Game Server : 고유번호와 토큰을 통해 로그인 요청
	Game Server -->> Fake Hive Server : 고유번호와 토큰의 유효성 검증 요청
	Fake Hive Server ->> Game Server : 유효성 검증
	alt 검증 실패
	Game Server -->> User : 로그인 실패 응답
	end
	
	Game Server ->> DB : 고유번호를 통해 유저 데이터 요청
	DB -->> Game Server : 유저 데이터 로드
	alt 존재하지 않는 유저
	Game Server -->> User : 로그인 실패 응답
	end
	Game Server -->> Game Server : 토큰을 Redis에 저장
	Game Server -->> User : 로그인 성공 응답
```