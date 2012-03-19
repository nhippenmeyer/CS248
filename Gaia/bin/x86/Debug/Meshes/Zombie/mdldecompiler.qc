$cd "C:\Users\Matt\Desktop\Anexia Dev Files\Models\Zombie\Extracted"
$modelname "Zombie/Fast.mdl"
$model "studio" "Zombiefast_reference.smd"
$lod 8
 {
  replacemodel "Zombiefast_reference" "lod1_Zombiefast_reference"
}
$lod 20
 {
  replacemodel "Zombiefast_reference" "lod2_Zombiefast_reference"
}
$lod 40
 {
  replacemodel "Zombiefast_reference" "lod3_Zombiefast_reference"
}
$lod 80
 {
  replacemodel "Zombiefast_reference" "lod4_Zombiefast_reference"
}
$shadowlod
{
  replacemodel "Zombiefast_reference" "lod5_Zombiefast_reference"
}
$model "headcrab1" "UnknownModelName.smd"
$lod 8
 {
  replacemodel "UnknownModelName" "lod1_UnknownModelName"
}
$lod 20
 {
  replacemodel "UnknownModelName" "lod2_UnknownModelName"
}
$lod 40
 {
  replacemodel "UnknownModelName" "lod3_UnknownModelName"
}
$lod 80
 {
  replacemodel "UnknownModelName" "lod4_UnknownModelName"
}
$shadowlod
{
  replacemodel "UnknownModelName" "lod5_UnknownModelName"
}
$model "headcrab1" "Headcrab_reference.smd"
$lod 8
 {
  replacemodel "Headcrab_reference" "lod1_Headcrab_reference"
}
$lod 20
 {
  replacemodel "Headcrab_reference" "lod2_Headcrab_reference"
}
$lod 40
 {
  replacemodel "Headcrab_reference" "lod3_Headcrab_reference"
}
$lod 80
 {
  replacemodel "Headcrab_reference" "lod4_Headcrab_reference"
}
$shadowlod
{
  replacemodel "Headcrab_reference" "lod5_Headcrab_reference"
}
$poseparameter move_yaw -180.00 180.00
$cdmaterials "models\zombie_fast\"
$cdmaterials "models\headcrab\"
$hboxset "default"
$hbox 0 "ValveBiped.Bip01_L_UpperArm" -4.650  -4.150  -2.870  14.950  3.120  2.130
$hbox 0 "ValveBiped.Bip01_L_Forearm" -2.500  -2.610  -1.730  12.710  1.550  2.130
$hbox 0 "ValveBiped.Bip01_L_Hand" -2.460  -1.300  -4.350  6.740  1.970  2.310
$hbox 0 "ValveBiped.Bip01_R_UpperArm" -3.370  -3.010  -2.850  14.890  3.630  2.160
$hbox 0 "ValveBiped.Bip01_R_Forearm" -2.260  -1.520  -1.690  12.990  2.870  2.160
$hbox 0 "ValveBiped.Bip01_R_Hand" -2.350  -1.440  -2.360  6.910  1.800  4.270
$hbox 0 "ValveBiped.Bip01_L_Thigh" -2.000  -5.330  -5.610  18.000  6.270  3.510
$hbox 0 "ValveBiped.Bip01_L_Calf" -3.880  -6.170  -4.630  22.020  4.230  4.240
$hbox 0 "ValveBiped.Bip01_R_Thigh" -2.000  -6.270  -5.610  18.000  5.330  3.510
$hbox 0 "ValveBiped.Bip01_R_Calf" -3.880  -4.230  -4.630  22.020  6.170  4.240
$hbox 1 "ValveBiped.HC_BodyCube" -6.250  -5.950  -5.480  6.030  8.210  4.940
$hbox 0 "ValveBiped.Bip01_Spine2" -2.000  -8.000  -4.000  18.000  8.000  8.000
$hbox 0 "ValveBiped.Bip01" -8.000  -4.500  -6.000  8.000  10.500  6.000
// Model uses material "fast_zombie_sheet.vmt"
// Model uses material "allinonebacup2.vmt"
$attachment "headcrab" "ValveBiped.HC_BodyCube" -0.00 -1.50 0.00 rotate 0.00 0.00 70.00
$attachment "head" "ValveBiped.HC_BodyCube" -0.00 0.00 0.00 rotate 0.00 0.00 0.00
$attachment "chest" "ValveBiped.Bip01_Spine4" -4.00 0.00 3.00 rotate -70.00 0.00 -180.00
$attachment "Blood_Left" "ValveBiped.Bip01_L_Finger2" 0.00 -0.00 -0.00 rotate -0.00 0.00 0.00
$attachment "Blood_Right" "ValveBiped.Bip01_R_Finger2" -0.00 0.00 0.00 rotate -0.00 0.00 -0.00
$attachment "Cable1a" "ValveBiped.Bip01_L_Forearm" 0.00 -0.00 0.00 rotate 0.00 0.00 0.00
$attachment "Cable1b" "ValveBiped.Bip01_L_Hand" 0.00 0.00 0.00 rotate 0.00 -0.00 0.00
$attachment "Cable2a" "ValveBiped.Bip01_R_Forearm" 0.00 -0.00 0.00 rotate -0.00 -0.00 0.00
$attachment "Cable2b" "ValveBiped.Bip01_R_Hand" 0.00 -0.00 -0.00 rotate 0.00 -0.00 0.00
$surfaceprop "flesh"
$keyvalues  { Cables { "Cable" { "StartAttachment" "Cable1a" "EndAttachment" "Cable1b" "Width" ".3" "Material" "cable/cable_lit" "NumSegments" "5" "Length" "115"  } "Cable" { "StartAttachment" "Cable2a" "EndAttachment" "Cable2b" "Width" ".3" "Material" "cable/cable_lit" "NumSegments" "5" "Length" "115"  }  } } 
$eyeposition -0.000 0.000 64.000
$illumposition 4.406 2.231 26.532
$sequence idle "idle" loop ACT_IDLE 1 fps 30.00
$sequence idle_angry "idle_angry" loop ACT_IDLE_ANGRY 1 fps 30.00
$sequence idle_onfire "idle_onfire" loop ACT_IDLE_ON_FIRE 1 fps 30.00
$sequence Run "Run" loop ACT_RUN 1 fps 30.00 {
  { event AE_FASTZOMBIE_GALLOP_LEFT 5 }
  { event AE_FASTZOMBIE_GALLOP_RIGHT 9 }
}

$sequence Leap "Leap" ACT_RANGE_ATTACK1 1 fps 30.00 {
  { event AE_FASTZOMBIE_LEAP 8 }
}

$sequence LeapStrike "LeapStrike" loop ACT_FASTZOMBIE_LEAP_STRIKE 1 fps 26.00
$sequence LandRight "LandRight" ACT_FASTZOMBIE_LAND_RIGHT 1 fps 30.00
$sequence LandLeft "LandLeft" ACT_FASTZOMBIE_LAND_LEFT 1 fps 30.00
$sequence Melee "Melee" loop ACT_MELEE_ATTACK1 1 fps 30.00 {
  { event AE_ZOMBIE_ATTACK_RIGHT 3 }
  { event AE_ZOMBIE_ATTACK_LEFT 7 }
}

$sequence climbmount "climbmount" fps 30.00
$sequence climbloop "climbloop" loop ACT_CLIMB_UP 1 fps 30.00 {
  { event AE_FASTZOMBIE_CLIMB_LEFT 5 }
  { event AE_FASTZOMBIE_CLIMB_RIGHT 12 }
}

$sequence climbdismount "climbdismount" ACT_CLIMB_DISMOUNT 1 fps 30.00
$sequence JumpNavMove "JumpNavMove" ACT_JUMP 1 fps 30.00
$sequence leap_start "leap_start" fps 30.00
$sequence leap_loop "leap_loop" loop fps 30.00
$sequence leap_land "leap_land" fps 30.00
$sequence BR2_Roar "BR2_Roar" ACT_FASTZOMBIE_FRENZY 1 fps 30.00
$sequence BR2_Attack "BR2_Attack" ACT_FASTZOMBIE_BIG_SLASH 1 fps 30.00 {
  { event AE_ZOMBIE_ATTACK_RIGHT 18 }
}

$animation a_Walks "a_Walks" loop fps 30.000000
$animation a_WalksE "a_WalksE" loop fps 30.000000
$animation a_WalkE "a_WalkE" loop fps 30.000000
$animation a_WalkNE "a_WalkNE" loop fps 30.000000
$animation a_WalkN "a_WalkN" loop fps 30.000000
$animation a_WalkNW "a_WalkNW" loop fps 30.000000
$animation a_WalkW "a_WalkW" loop fps 30.000000
$animation a_WalkSW "a_WalkSW" loop fps 30.000000
$sequence walk_all "a_Walks" loop ACT_WALK 1 fps 30.00 {
  { event AE_ZOMBIE_STEP_RIGHT 7 }
  { event AE_ZOMBIE_STEP_LEFT 16 }
  blendwidth 9
  blend move_yaw -180.000000 180.000000
 a_WalksE a_WalkE a_WalkNE a_WalkN a_WalkNW a_WalkW a_WalkSW a_Walks
}
$sequence ragdoll "ragdoll" ACT_DIERAGDOLL 1 fps 30.00
$ikchain rfoot ValveBiped.Bip01_R_Foot knee  0.774 -0.633 0.000
$ikchain lfoot ValveBiped.Bip01_L_Foot knee  0.000 0.000 0.000
$ikchain rhand ValveBiped.Bip01_R_Hand knee  0.759 0.651 -0.000
$ikchain lhand ValveBiped.Bip01_L_Hand knee  0.726 0.688 0.000
$collisionjoints "phymodel.smd" {

	$mass 95.0
	$inertia 2.00
	$damping 0.01
	$rotdamping 0.40
	$rootbone "valvebiped.bip01_pelvis"

	$jointconstrain "valvebiped.bip01_spine" x limit -32.00 32.00 0.20
	$jointconstrain "valvebiped.bip01_spine" y limit -39.00 39.00 0.20
	$jointconstrain "valvebiped.bip01_spine" z limit -15.00 40.00 0.20

	$jointconstrain "valvebiped.bip01_spine2" x limit 0.00 0.00 0.00
	$jointconstrain "valvebiped.bip01_spine2" y limit 0.00 0.00 0.20
	$jointconstrain "valvebiped.bip01_spine2" z limit -41.00 41.00 0.20

	$jointconstrain "valvebiped.bip01_l_upperarm" x limit -30.00 61.00 0.20
	$jointconstrain "valvebiped.bip01_l_upperarm" y limit -90.00 48.00 0.20
	$jointconstrain "valvebiped.bip01_l_upperarm" z limit -106.00 68.00 0.20

	$jointmassbias "valvebiped.bip01_l_forearm" 2.00
	$jointconstrain "valvebiped.bip01_l_forearm" x limit 1.00 1.00 0.20
	$jointconstrain "valvebiped.bip01_l_forearm" y limit 0.00 0.00 0.20
	$jointconstrain "valvebiped.bip01_l_forearm" z limit -102.00 44.00 0.20

	$jointmassbias "valvebiped.bip01_l_hand" 2.00
	$jointconstrain "valvebiped.bip01_l_hand" x limit -39.00 39.00 0.20
	$jointconstrain "valvebiped.bip01_l_hand" y limit -19.00 19.00 0.20
	$jointconstrain "valvebiped.bip01_l_hand" z limit -64.00 26.00 0.20

	$jointconstrain "valvebiped.bip01_r_upperarm" x limit -41.00 37.00 0.20
	$jointconstrain "valvebiped.bip01_r_upperarm" y limit -44.00 90.00 0.20
	$jointconstrain "valvebiped.bip01_r_upperarm" z limit -106.00 68.00 0.20

	$jointmassbias "valvebiped.bip01_r_forearm" 2.00
	$jointconstrain "valvebiped.bip01_r_forearm" x limit 0.00 0.00 0.20
	$jointconstrain "valvebiped.bip01_r_forearm" y limit 0.00 0.00 0.00
	$jointconstrain "valvebiped.bip01_r_forearm" z limit -41.00 93.00 0.20

	$jointconstrain "valvebiped.bip01_r_thigh" x limit -28.00 30.00 0.20
	$jointconstrain "valvebiped.bip01_r_thigh" y limit -37.00 26.00 0.20
	$jointconstrain "valvebiped.bip01_r_thigh" z limit -82.00 44.00 0.20

	$jointmassbias "valvebiped.bip01_r_calf" 2.00
	$jointconstrain "valvebiped.bip01_r_calf" x limit 0.00 0.00 0.20
	$jointconstrain "valvebiped.bip01_r_calf" y limit 0.00 0.00 0.20
	$jointconstrain "valvebiped.bip01_r_calf" z limit -8.00 130.00 0.20

	$jointmassbias "valvebiped.bip01_r_foot" 2.00
	$jointconstrain "valvebiped.bip01_r_foot" x limit -30.00 30.00 0.20
	$jointconstrain "valvebiped.bip01_r_foot" y limit -28.00 28.00 0.20
	$jointconstrain "valvebiped.bip01_r_foot" z limit -30.00 26.00 0.20

	$jointconstrain "valvebiped.bip01_l_thigh" x limit -37.00 30.00 0.20
	$jointconstrain "valvebiped.bip01_l_thigh" y limit -30.00 19.00 0.20
	$jointconstrain "valvebiped.bip01_l_thigh" z limit -75.00 39.00 0.20

	$jointmassbias "valvebiped.bip01_l_calf" 2.00
	$jointconstrain "valvebiped.bip01_l_calf" x limit 0.00 0.00 0.00
	$jointconstrain "valvebiped.bip01_l_calf" y limit 0.00 0.00 0.00
	$jointconstrain "valvebiped.bip01_l_calf" z limit -8.00 130.00 0.20

	$jointmassbias "valvebiped.bip01_l_foot" 2.00
	$jointconstrain "valvebiped.bip01_l_foot" x limit -10.00 10.00 0.20
	$jointconstrain "valvebiped.bip01_l_foot" y limit -17.00 17.00 0.20
	$jointconstrain "valvebiped.bip01_l_foot" z limit -21.00 39.00 0.20

	$jointmassbias "valvebiped.bip01_r_hand" 2.00
	$jointconstrain "valvebiped.bip01_r_hand" x limit -58.00 58.00 0.20
	$jointconstrain "valvebiped.bip01_r_hand" y limit -28.00 28.00 0.20
	$jointconstrain "valvebiped.bip01_r_hand" z limit -35.00 46.00 0.20
}
