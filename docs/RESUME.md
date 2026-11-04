# TodoApi CI/CD 实战 — 简历素材包

> 用法：第一、二节贴简历；第三节面试口述（每个 1 分钟）；第四节是临场心法。

## 一、项目描述（贴简历用）

**TodoApi —— CI/CD 全流程实战（个人项目）**
C# / .NET 10 Minimal API · xUnit · GitHub Actions · Docker · GHCR · Self-hosted Runner · Render

- 从零构建四阶段 CI/CD 流水线：**测试门禁**（单元+集成测试、格式检查、80% 覆盖率棘轮）→ **多阶段 Docker 构建**（700MB→120MB，应用层仅 152kB）→ **GHCR 双标签推送**（latest + sha）→ **审批后本机与云端并行部署**，端到端约 3 分钟
- 通过分支保护（Rulesets：强制 PR + 必需状态检查）实现"**红灯物理阻断合并**"，主干永远可发布
- 工程优化：NuGet/Docker 双缓存体系（哈希 key 设计 + restore-keys 兜底 + gha 缓存）、Deploy Hook 触发云部署（secrets 保管）、幂等部署脚本
- 独立排查修复 4 起真实故障，沉淀"现象→定位→根因→修复→原则"排障方法论

## 二、技能关键词（对照 JD 筛词）

CI/CD · GitHub Actions · Docker 多阶段构建 · GHCR · 分支保护 · xUnit 单元/集成测试 · 覆盖率门禁（coverlet）· Self-hosted Runner · GITHUB_TOKEN 最小权限 · Secrets 管理 · 幂等部署 · Deploy Hook · 回滚策略（sha 双标签）· YAML 排障 · Git 协作流（PR/Squash/Conventional Commits）

## 三、四个 STAR 故事（面试口述版）

### 故事 1：9 秒失败的流水线（缓存设计）

- **S**：给流水线加 NuGet 缓存，选了 `setup-dotnet` 的 `cache: true` 捷径
- **T**：主干流水线 9 秒即失败，需快速定位
- **A**：从"失败耗时极短"判断死在初始化阶段（任何 dotnet 命令都没跑）；查日志发现该选项依赖 `packages.lock.json` 锁文件，而 .NET 项目默认不启用锁文件模式；改用 `actions/cache` 手动管理，key 用 csproj 哈希，补 `restore-keys` 前缀兜底
- **R**：缓存命中、依赖还原提速。沉淀："封装有假设，手写更可控"；缓存 key 设计遵循"变则换、不变则复用"

### 故事 2：参数跑错家门（YAML 排障）

- **S**：CI 新增 docker job 后主干红灯
- **T**：buildx 报 `invalid tag "cache-from: type=gha"`
- **A**：识别关键信号——**参数名出现在了不可能的位置**（被当成镜像标签）；反推 YAML 多行块（`|`）按缩进圈地，`cache-from` 缩进过深被吞进 tags 列表；调整缩进使其与 `tags:` 对齐
- **R**：转绿。沉淀 YAML 排障直觉："参数错位，先查缩进"

### 故事 3：休眠到第二次部署才爆（幂等与命名）

- **S**：部署 job 含"停旧容器→启新容器"两步，用 `|| true` 做幂等清理
- **T**：首次部署成功，第二次部署撞名失败（容器已存在）
- **A**：对比两步发现 `docker rm` 的名字（todo-api）与 `docker run --name` 的名字（todo-api-prod）**漂移**；`|| true` 把"删错对象"的报错也吞掉了，bug 休眠到第二次部署；将容器名收敛为 job 级环境变量，清理与创建永远同名
- **R**：部署可安全重复执行。沉淀：幂等清理必须保证目标同名；多处出现的名字用变量收敛（DRY）

### 故事 4：全绿流水线，云端没更新（触发链闭环）

- **S**：CI 全绿、新镜像已推 GHCR，Render 云端仍返回旧版本
- **T**：找出部署为何没生效
- **A**：排查"**谁负责通知 Render 更新**"——平台侧 Auto-Deploy 不可用、流水线侧无触发 job，触发链断裂；先用 Manual Deploy 验证链路，再创建 Deploy Hook 存入 GitHub Secrets（`RENDER_DEPLOY_HOOK`），流水线增加 deploy-cloud job 主动 curl 钩子
- **R**：云端与本机并行自动部署，双端同步零人手。沉淀：部署触发必须显式闭环——轮询/钩子/手动三选一，不能空缺

## 四、临场心法

- 万能公式：**为什么（动机）→ 怎么做（机制 + 数字）→ 踩过什么坑（亲历）**
- 三处硬伤重背：① job 改名必须同步更新分支保护的必需检查（否则 PR 永久卡死）② runner 安全："PR 类 job 只跑云端 runner，self-hosted 只接 push 触发的部署" ③ main 可发布 ≠ 禁止半成品合入——**功能开关（Feature Flag）**让代码进主干而功能对用户关闭
- 数字弹药库：4+1 测试 / 覆盖率 100% 阈值 80% / 700MB→120MB / 应用层 152kB / 端到端 3 分钟 / 流水线 30-40 秒（部署全程约 2 分半）
