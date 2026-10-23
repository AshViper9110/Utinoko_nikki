<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class FamilyUserSeeder extends Seeder
{
    public function run(): void
    {
        DB::table('family_users')->insert([
            [
                'name' => '山田子郎',
                'firebase_uid' => 'firebase_test_uid_002',
            ],
        ]);
    }
}
