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

## 8. 变基冲突处理手册（zkt 分支同步 main）

适用场景
- 你在个人分支（例如 zkt）开发，需要同步最新 main。

标准流程（无冲突）
1. git fetch origin
2. git rebase origin/main
3. git push --force-with-lease origin zkt

发生冲突时（通用）
1. 先看冲突文件
	- git status
2. 打开冲突文件，处理标记
	- <<<<<<<
	- =======
	- >>>>>>>
3. 处理完成后标记已解决
	- git add <文件路径>
4. 继续变基
	- git rebase --continue
5. 如果还有冲突，重复 1-4，直到结束
6. 完成后推送
	- git push --force-with-lease origin zkt

Unity 场景冲突专用处理（.unity / .prefab / .asset）
1. 不建议直接在文本里硬改大段 YAML。
2. 冲突复杂时优先策略
	- 先保留一方场景版本完成变基
	- 在 Unity Editor 中手工补回另一方改动
3. 场景文件与对应 meta 要同时关注。
4. 完成后务必在 Unity 中验证
	- 无 Missing 脚本
	- 关键流程可跑通

常用快速命令
1. 查看当前卡在哪个提交
	- git rebase --show-current-patch
2. 跳过当前冲突提交（谨慎）
	- git rebase --skip
3. 放弃本次变基，回到变基前
	- git rebase --abort

强制要求（避免踩坑）
1. 不要在变基进行中执行新的 pull。
2. 不要在未解决冲突前直接 push。
3. 变基后推送必须用 --force-with-lease，不要用 --force。

最短应急流程（可直接照抄）
1. git fetch origin
2. git rebase origin/main
3. 发生冲突后：
	- git status
	- 解决冲突
	- git add .
	- git rebase --continue
4. 全部完成后：
	- git push --force-with-lease origin zkt
