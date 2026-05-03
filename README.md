<!-- README.md -->

<p align="center">
  <img src="docs/images/ham-cheeto-banner.png" width="780" alt="햄치토 Ham-Cheeto 배너" />
</p>

<h1 align="center">🐹 햄치토 Ham-Cheeto 🧀</h1>

<p align="center">
  <b>귀여운 햄스터 셰프와 함께하는 토스트 요리 타이쿤 게임</b>
</p>

<p align="center">
  손님의 주문에 맞춰 재료를 조리하고, 토스트를 완성해 목표 금액을 달성하세요!
</p>

<p align="center">
  <img src="https://img.shields.io/badge/Engine-Unity%206000.3.9f1-black?logo=unity" />
  <img src="https://img.shields.io/badge/Genre-Cooking%20Tycoon-orange" />
  <img src="https://img.shields.io/badge/Platform-PC-blue" />
  <img src="https://img.shields.io/badge/Status-In%20Development-yellow" />
</p>

---

## 📌 목차

- [🍞 게임 소개](#-게임-소개)
- [🎮 게임 흐름](#-게임-흐름)
- [🧱 기본 구조](#-기본-구조)
- [✨ 주요 구현 기능](#-주요-구현-기능)
- [🛠️ 문제 해결 경험](#️-문제-해결-경험)
- [📸 플레이 화면](#-플레이-화면)
- [💻 개발 환경](#-개발-환경)
- [📁 프로젝트 구조](#-프로젝트-구조)
- [🚀 향후 개발 계획](#-향후-개발-계획)

---

## 🍞 게임 소개

**햄치토 Ham-Cheeto**는 손님의 주문에 맞춰 재료를 조리하고 토스트를 완성하는  
**요리 시뮬레이션 / 타이쿤 게임**입니다.

플레이어는 제한 시간 안에 다양한 재료를 조합해 토스트를 만들고,  
손님에게 제공하여 목표 금액을 달성해야 합니다.

> 깔끔하고 부드러운 그래픽과 자연스러운 조작감을 중심으로,  
> 누구나 쉽고 재미있게 즐길 수 있는 요리 시뮬레이션을 목표로 개발했습니다.

<br />

| 항목 | 내용 |
|---|---|
| 🎮 장르 | 요리, 타이쿤 |
| 🎯 타겟 유저 | 요리 시뮬레이션을 좋아하는 전연령대 유저 |
| 💡 개발 포인트 | 깔끔하고 부드러운 그래픽 및 모션 |
| 🗓️ 개발 기간 | 2026.04.08 ~ 2026.04.26 |
| 🛠️ 개발 환경 | Unity 6000.3.9f1 |

---

## 🎮 게임 흐름

```txt
손님 등장
   ↓
주문 확인
   ↓
재료 조리
   ↓
토스트 완성
   ↓
포장 후 손님에게 제공
   ↓
수익 획득
   ↓
제한 시간 내 목표 금액 달성
```

### 전체적인 게임 목표

1. **손님의 주문서에 맞춰 재료를 조리하고 토스트 완성하기**
2. **완성한 토스트를 포장하고 손님에게 제공하기**
3. **제한 시간 내에 목표 금액을 달성하여 게임 클리어하기**

---

## 🧱 기본 구조

게임은 크게 **싱글톤 매니저 구조**, **클릭 기반 조작 시스템**,  
그리고 **재료 / 토스트 / 주문 데이터 관리 구조**를 중심으로 구성했습니다.

### 🔹 싱글톤 매니저

| 매니저 | 역할 |
|---|---|
| `GameManager` | 게임 기본 진행 조건 설정 및 관리 |
| `SoundManager` | BGM, SFX 재생 |
| `CursorController` | 커서 이미지 및 액션 상태 관리 |

```txt
GameManager
 ├─ 제한 시간 관리
 ├─ 목표 금액 관리
 └─ 체력 / 게임 상태 관리

SoundManager
 ├─ BGM 재생
 └─ SFX 재생

CursorController
 ├─ 마우스 커서 이미지 변경
 └─ PickUp / Drop 액션 처리
```

---

### 🔹 클릭 기반 조작

게임 내 상호작용 가능한 오브젝트는  
공통 부모 클래스인 `ClickableObject`를 상속하도록 구성했습니다.

```txt
ClickableObject
 ├─ Ingredient
 ├─ Grill
 ├─ PackagingArea
 └─ Customer
```

마우스 클릭 시점을 `EventSystem`에서 받아오고,  
각 오브젝트는 상황에 맞게 `PickUp` / `Drop` 액션을 수행합니다.

> 이를 통해 재료 집기, 재료 내려놓기, 토스트 포장 등  
> 다양한 상호작용을 일관된 방식으로 처리할 수 있도록 구성했습니다.

---

## ✨ 주요 구현 기능

### 🥚 1. 재료 상태 변경 로직

각 재료는 `Type`과 `State`를 `Enum`으로 관리합니다.

재료가 특정 오브젝트, 예를 들어 `Grill`의 `Collider` 내부에 배치되면  
별도의 리스트에 등록되고, `Update`에서 조리 시간을 누적합니다.

```csharp
public enum IngredientType
{
    Bread,
    Ham,
    Cheese,
    Egg,
    Sauce
}

public enum IngredientState
{
    Raw,
    Cooking,
    Cooked,
    Burnt
}
```

조리 시간이 정해진 기준에 도달하면 재료의 상태가 변경됩니다.

```txt
Raw → Cooking → Cooked → Burnt
```

상태 변경 시에는 다음 요소도 함께 변경됩니다.

- 재료 스프라이트 변경
- 조리 효과음 재생
- 주문 판정에 사용할 데이터 갱신

---

### 🥪 2. 아이템 조합 로직

토스트는 여러 재료를 순서대로 쌓아 완성합니다.

게임 화면 내 포장지 `Collider` 영역 안에서만  
빵과 재료를 배치할 수 있도록 제한했습니다.

```txt
포장지 영역
 └─ toastStack
     ├─ Bread
     ├─ Ham
     ├─ Cheese
     └─ Bread
```

구현 핵심은 다음과 같습니다.

- 재료를 쌓을 수 있는 `toastStack` 리스트 생성
- 빵을 배치하면 `toastStack[0]`이 빵으로 설정
- 재료가 추가될수록 위쪽으로 쌓이도록 위치 보정
- UX를 위해 포장지 `Collider` 크기를 동적으로 확장
- `Space` 키 입력 시 현재 토스트 데이터를 복사하여 포장 처리

```csharp
Dictionary<int, IngredientData> toastData = new Dictionary<int, IngredientData>();
List<Ingredient> toastStack = new List<Ingredient>();
```

---

### 🐱 3. 손님 응대 로직

손님의 주문은 `CSV` 파일을 기반으로 관리했습니다.

레시피 데이터를 행 / 열 단위로 분리하여  
약 20개의 주문 가능한 레시피를 `List`와 `Dictionary`에 등록합니다.

```txt
Recipe CSV
 ├─ 구운빵
 ├─ 치즈
 ├─ 햄
 ├─ 계란
 └─ 소스
```

손님이 스폰되면 등록된 레시피 중 하나를 랜덤으로 선택합니다.

```csharp
currentOrder = recipeList[Random.Range(0, recipeList.Count)];
```

이후 플레이어가 완성한 토스트 데이터와  
손님의 현재 주문 데이터를 비교하여 성공 / 실패를 판정합니다.

```txt
currentOrder == toastData
        ↓
주문 성공 / 실패 판정
```

---

### 💰 4. 시간과 목표 금액 시스템

플레이어는 제한 시간 안에 목표 금액을 달성해야 합니다.

```txt
남은 시간 01:05
오늘 번 돈 2000
목표 금액 20000원
```

게임 진행 중 관리되는 주요 값은 다음과 같습니다.

| 요소 | 설명 |
|---|---|
| 남은 시간 | 스테이지 클리어 제한 시간 |
| 오늘 번 돈 | 주문 성공 시 증가하는 수익 |
| 목표 금액 | 클리어 조건 |
| 체력 | 주문 실패 또는 실수 시 감소 가능 |

---

## 🛠️ 문제 해결 경험

개발 과정에서 발생한 주요 문제와 해결 방식입니다.

### 1. 씬 전환 시 오브젝트 참조 누락

씬 전환 시 버튼 `Action`과 `AudioSource`에서  
`Missing Reference` 문제가 발생했습니다.

**해결 방법**

- 씬마다 별도의 오브젝트를 배치
- `SoundBridge`, `SceneLoadBridge`를 사용해 싱글톤 의존성 감소
- 버튼이 직접 싱글톤을 참조하지 않도록 구조 개선

---

### 2. 소스류 재료 배치 문제

처음에는 소스류를 일반 `Ingredient`로 지정했으나,  
소스를 “들어 올리는” 액션이 부자연스럽게 느껴졌습니다.

**해결 방법**

- 소스는 빵 위에만 배치 가능하도록 제한
- 소스의 상태는 별도의 `bool` 값으로 제어
- 일반 재료와 다른 방식의 배치 로직 적용

---

### 3. Sorting Order 처리 문제

마우스로 집은 오브젝트를 앞으로 가져오는 과정에서  
UI와 월드 오브젝트 간 렌더링 순서 문제가 발생했습니다.

**해결 방법**

- 집은 오브젝트의 `Sorting Order`를 임시로 증가
- Drop 시 기존 레이어 순서로 복구
- UI와 월드 오브젝트의 표시 영역을 구분

---

### 4. 좌표계 혼동 문제

`Screen Space - Overlay`, `Camera`, `World Space` 간의 차이로 인해  
오브젝트가 의도치 않은 위치로 이동하는 문제가 있었습니다.

**해결 방법**

- UI와 월드 오브젝트의 좌표 변환 방식을 분리
- `Camera.ScreenToWorldPoint()` 사용 위치를 명확히 구분
- 하위 오브젝트의 로컬 좌표와 월드 좌표를 분리하여 처리

---

## 📸 플레이 화면

### 🧀 타이틀 화면

<p align="center">
  <img src="docs/images/title.png" width="700" alt="햄치토 타이틀 화면" />
</p>

---

### 🍞 기본 조작 화면

<p align="center">
  <img src="docs/images/gameplay-toast.png" width="700" alt="토스트 조작 화면" />
</p>

---

### 🥚 재료 조리

<p align="center">
  <img src="docs/images/cooking-ingredients.png" width="700" alt="재료 조리 화면" />
</p>

---

### 📦 토스트 포장

<p align="center">
  <img src="docs/images/packaging-toast.png" width="700" alt="토스트 포장 화면" />
</p>

---

### 🐱 손님 응대

<p align="center">
  <img src="docs/images/customer-order.png" width="700" alt="손님 주문 화면" />
</p>

---

## 💻 개발 환경

| 구분 | 내용 |
|---|---|
| Engine | Unity 6000.3.9f1 |
| Language | C# |
| Data | Excel CSV |
| Version Control | Git / GitHub |
| Platform | PC |

---

## 📁 프로젝트 구조

```txt
Assets/
├─ Scripts/
│  ├─ Managers/
│  │  ├─ GameManager.cs
│  │  ├─ SoundManager.cs
│  │  └─ CursorController.cs
│  │
│  ├─ Objects/
│  │  ├─ ClickableObject.cs
│  │  ├─ Ingredient.cs
│  │  ├─ Grill.cs
│  │  └─ Customer.cs
│  │
│  ├─ Data/
│  │  ├─ IngredientData.cs
│  │  └─ RecipeData.cs
│  │
│  └─ UI/
│     ├─ TimerUI.cs
│     └─ MoneyUI.cs
│
├─ Scenes/
├─ Prefabs/
├─ Sprites/
├─ Sounds/
└─ Data/
   └─ recipes.csv
```

---

## 🚀 향후 개발 계획

- [ ] 손님별 특성 추가
- [ ] 주문 확률 조정 시스템 추가
- [ ] 이벤트 / 버프 시스템 추가
- [ ] 레벨 업에 따른 스테이지 전환
- [ ] 난이도별 목표 금액 조정
- [ ] 새로운 재료 및 레시피 추가
- [ ] 튜토리얼 UI 추가
- [ ] 결과 화면 개선

---

## 🧡 프로젝트 한 줄 소개

> **햄치토**는 귀여운 햄스터 셰프와 함께  
> 재료를 굽고, 쌓고, 포장하며 손님을 만족시키는  
> 아기자기한 요리 타이쿤 게임입니다.

---

## 📂 이미지 파일 배치 안내

README에서 이미지가 보이게 하려면 프로젝트에 아래처럼 이미지를 넣어주세요.

```txt
docs/
└─ images/
   ├─ ham-cheeto-banner.png
   ├─ title.png
   ├─ gameplay-toast.png
   ├─ cooking-ingredients.png
   ├─ packaging-toast.png
   └─ customer-order.png
```

| 파일명 | 추천 이미지 |
|---|---|
| `ham-cheeto-banner.png` | 햄치토 로고와 햄스터가 있는 대표 이미지 |
| `title.png` | 타이틀 화면 |
| `gameplay-toast.png` | 빵을 잡고 조작하는 기본 화면 |
| `cooking-ingredients.png` | 계란, 햄, 치즈 조리 화면 |
| `packaging-toast.png` | 포장지 위에 토스트를 올린 화면 |
| `customer-order.png` | 손님 주문 및 목표 금액 화면 |
