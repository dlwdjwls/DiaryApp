# 일기장 웹앱 디자인/스타일링 설계

## 배경
일기장 웹앱의 기능(회고 홈 `/`, 새 일기 작성 `/new`, 목록·검색 `/timeline`)은 이미 구현되어 있으나,
스타일은 Blazor Server 기본 템플릿의 최소 `wwwroot/app.css`뿐이라 꾸미지 않은 HTML 그대로 보인다.
이 문서는 화면에 실제 디자인(색상, 타이포그래피, 레이아웃, 컴포넌트 스타일)을 입히는 작업을 다룬다.

## 목표
- 차분하고 진지한 다크 톤의 개인 일기장다운 인상을 준다.
- 기존 기능/마크업 구조는 건드리지 않고 스타일만 새로 입힌다 (별도 CSS 프레임워크나 빌드 도구 도입 없음).

## 범위 밖
- 라이트 모드/다크 모드 토글 (다크 톤 고정)
- 애니메이션·트랜지션 등 동적 효과
- 반응형 모바일 최적화(기존처럼 브라우저 기본 반응형 정도만 유지, 별도 브레이크포인트 설계 없음)

## 구현 방식
`src/DiaryApp/wwwroot/app.css` 하나에 CSS 커스텀 프로퍼티(변수)와 컴포넌트별 스타일을 추가한다.
새 파일이나 프레임워크, 빌드 파이프라인은 도입하지 않는다 (화면 3개짜리 개인 프로젝트 규모에 맞춤 —
YAGNI). 마크업(Razor 파일)에는 스타일링에 필요한 최소한의 CSS 클래스만 추가한다.

## 색상 팔레트 (CSS 변수)
```css
--bg: #10141c;           /* 페이지 배경 */
--surface: #1a212c;      /* 카드/네비게이션 표면 */
--border: #2a3342;       /* 테두리 */
--text: #e7ebf2;         /* 기본 텍스트 */
--text-muted: #8b96a8;   /* 보조 텍스트 (날짜, 태그 등) */
--accent: #6c8fc7;       /* 포인트 컬러 (버튼, 링크, 포커스) */
--accent-hover: #8aa6d1; /* 포인트 컬러 호버 */
--danger: #d9736a;       /* 폼 에러 메시지 */
```

## 타이포그래피
- 폰트: `-apple-system, BlinkMacSystemFont, "Segoe UI", "Malgun Gothic", sans-serif` (시스템 산세리프, 한글 포함)
- 본문: 1rem / line-height 1.6
- h1: 1.75rem, h2: 1.25rem
- 보조 텍스트(날짜, 태그): 0.875rem

## 레이아웃 & 내비게이션
- 전체 콘텐츠는 화면 중앙 정렬, `max-width: 680px`, 좌우 자동 여백
- 상단 내비게이션: `--surface` 배경 + 하단 1px `--border`, "회고 · 새 일기 · 목록·검색" 링크를 여백을 두고 가로 배치
- 현재 라우트에 해당하는 링크는 `--accent` 색 + 밑줄로 강조 (Blazor의 `NavLink`/`Match` 활용)
- 섹션·카드 사이 여백을 넉넉히 둬 답답하지 않게 한다

## 컴포넌트 스타일

**일기 카드** (Home의 회고 항목, Timeline의 목록 항목 공통)
- `--surface` 배경, `border-radius: 8px`, 1px `--border` 테두리, 내부 패딩, 카드 간 하단 마진
- 날짜는 `--text-muted`, 본문은 `--text`

**태그**
- 알약(pill) 모양 배지: `border-radius: 999px`, `--border` 배경, `--text-muted` 텍스트, 작은 패딩

**기분**
- 이모티콘을 날짜 옆에 `font-size: 1.25em` 정도로 살짝 크게 표시 (별도 배지 없이 이모티콘 자체로 표현)

**이미지**
- 카드 하단에 썸네일 grid (`display: flex; flex-wrap: wrap; gap`), 각 이미지 `border-radius: 6px`,
  `object-fit: cover`로 정사각형 비율 크롭

**새 일기 작성 폼**
- `input`, `textarea`, `select`: `--surface` 배경 + `--border` 테두리, `:focus` 시 테두리가 `--accent`로 변경
- 저장 버튼: `--accent` 배경의 강조 버튼, 호버 시 `--accent-hover`
- 에러 메시지: `--danger` 텍스트

## 적용 대상 파일
- `src/DiaryApp/wwwroot/app.css` — 전체 스타일 정의 (수정)
- `src/DiaryApp/Components/Layout/MainLayout.razor` — 내비게이션에 스타일링용 클래스/현재 라우트 강조 추가
- `src/DiaryApp/Components/Pages/Home.razor` — 카드/태그/기분 클래스 적용
- `src/DiaryApp/Components/Pages/Timeline.razor` — 카드/태그/기분/이미지 클래스 적용
- `src/DiaryApp/Components/Pages/NewEntry.razor` — 폼 요소 클래스 적용

## 검증
- 브라우저로 각 화면(`/`, `/new`, `/timeline`)을 열어 색상·레이아웃·컴포넌트 스타일이 설계대로 보이는지 확인
- 폼 입력 포커스 시 테두리 색 변경, 저장 버튼 호버 효과 확인
