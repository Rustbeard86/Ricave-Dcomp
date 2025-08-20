using System;

namespace Ricave.Core
{
    public class Instruction_RegisterKilledBoss : Instruction
    {
        public EntitySpec ActorSpec
        {
            get
            {
                return this.actorSpec;
            }
        }

        public string Name
        {
            get
            {
                return this.name;
            }
        }

        public int Floor
        {
            get
            {
                return this.floor;
            }
        }

        public int RampUp
        {
            get
            {
                return this.rampUp;
            }
        }

        protected Instruction_RegisterKilledBoss()
        {
        }

        public Instruction_RegisterKilledBoss(Actor boss, int floor)
        {
            this.actorSpec = boss.Spec;
            this.name = (boss.Name.NullOrEmpty() ? boss.Spec.LabelCap : boss.Name);
            this.floor = floor;
            this.rampUp = boss.RampUp;
        }

        protected override void DoImpl()
        {
            this.killedBossIndex = Get.KillCounter.RegisterKilledBoss(this.actorSpec, this.name, this.floor, this.rampUp, DateTime.UtcNow);
        }

        protected override void UndoImpl()
        {
            Get.KillCounter.DeregisterKilledBoss(this.killedBossIndex);
        }

        [Saved]
        private EntitySpec actorSpec;

        [Saved]
        private string name;

        [Saved]
        private int floor;

        [Saved]
        private int rampUp;

        [Saved]
        private int killedBossIndex;
    }
}