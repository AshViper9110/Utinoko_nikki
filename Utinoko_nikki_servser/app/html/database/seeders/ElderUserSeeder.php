<?php

namespace Database\Seeders;

use Illuminate\Database\Seeder;
use Illuminate\Support\Facades\DB;

class ElderUserSeeder extends Seeder
{
    public function run(): void
    {
        DB::table('elder_users')->insert([
            [
                'name' => '山田太郎',
                'firebase_uid' => 'firebase_test_uid_001',
            ],
        ]);
    }
}
