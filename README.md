# Unity2D 三人协作指南（My-RPG）

本仓库是 Unity2D 小游戏项目，Unity 工程目录在 [My-RPG](My-RPG)。

## 1. 必传与不传

必须提交（项目可复现的核心）
- [My-RPG/Assets](My-RPG/Assets)
- [My-RPG/Packages](My-RPG/Packages)
- [My-RPG/ProjectSettings](My-RPG/ProjectSettings)
- [My-RPG/My-RPG.sln](My-RPG/My-RPG.sln)
- 根目录协作文件（[.gitignore](.gitignore)、[.gitattributes](.gitattributes)、本 README）

必须忽略（Unity 自动生成缓存）
- [My-RPG/Library](My-RPG/Library)
- [My-RPG/Temp](My-RPG/Temp)
- [My-RPG/Logs](My-RPG/Logs)
- [My-RPG/UserSettings](My-RPG/UserSettings)

注意
- `Assets` 下的 `.meta` 文件必须和资源一起提交，否则会丢失资源引用。

## 2. 首次初始化（仓库创建者）

在仓库根目录执行：

```powershell
git add .gitignore .gitattributes README.md
git add My-RPG/Assets My-RPG/Packages My-RPG/ProjectSettings My-RPG/My-RPG.sln
git commit -m "chore: initialize unity repository structure"
git branch -M main
git push -u origin main
```

如果你们要使用 Git LFS（推荐）：

```powershell
git lfs install
git lfs track "*.png" "*.jpg" "*.psd" "*.wav" "*.fbx" "*.mp4"
git add .gitattributes
git commit -m "chore: enable git lfs for binary assets"
git push
```

## 3. 三人分支协作模型

分支设计
- `main`：稳定可运行版本（仅发布/里程碑）
- `develop`：日常集成分支
- `feature/<name>-<module>`：个人功能分支，例如 `feature/lin-ui`

创建 `develop`：

```powershell
git checkout -b develop
git push -u origin develop
```

每位同学从 `develop` 切自己的功能分支：

```powershell
git checkout develop
git pull
git checkout -b feature/<name>-<module>
```

## 4. 每日协作流程（统一执行）

1. 开工前同步：

```powershell
git checkout develop
git pull
git checkout feature/<name>-<module>
git merge develop
```

2. 本地开发并小步提交：

```powershell
git add .
git commit -m "feat: <简要描述>"
git push -u origin feature/<name>-<module>
```

3. 发起 PR：`feature/* -> develop`
- 至少 1 人 Review
- 确认 Unity 可打开、关键场景可运行

4. 阶段发布：`develop -> main`

```powershell
git checkout main
git pull
git merge develop
git tag v0.1.0
git push
git push --tags
```

## 5. 冲突避免与场景协作规则

- 尽量不要两个人同时改同一个 `.unity` 场景文件。
- 复杂场景尽量拆成多个 Prefab，让不同同学改不同 Prefab。
- 代码和资源分工尽量固定，降低冲突概率：
	- A：核心玩法/数值/AI
	- B：UI/交互流程
	- C：关卡/美术资源/动画

发生冲突时：
- 优先在 Unity Editor 内确认最终效果，再提交解决结果。
- 场景冲突很复杂时，按“保留一方场景 + 手工重做另一方少量改动”的策略通常更稳。

## 6. 提交信息规范（建议）

- `feat:` 新功能
- `fix:` 缺陷修复
- `refactor:` 重构
- `chore:` 工程配置与维护
- `docs:` 文档更新

示例：
- `feat: add player dash skill`
- `fix: prevent enemy attack null reference`
- `chore: tune project quality settings`

## 7. 快速检查清单（提交前）

- Unity Console 无新的 Error
- 关键场景可进入、可操作、可退出
- 没有把 [My-RPG/Library](My-RPG/Library) / [My-RPG/Temp](My-RPG/Temp) 提交进来
- 新增资源是否带 `.meta`
